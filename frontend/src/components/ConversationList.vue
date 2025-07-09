<script setup lang="ts">
	import { storeToRefs } from "pinia";
	import { useConversationStore } from "../stores/conversationStore";
	import { useUserStore } from "../stores/userStore";
	import { useField, useForm } from "vee-validate";
	import { startConversationSchema } from "../schemas/conversationSchema";
	import { useFetch } from "@vueuse/core";
	import { ref } from "vue";
	import type { Conversation } from "../types";
	import { getSignalR } from "../lib/hub";
import { RouterLink } from "vue-router";

	const conversationStore = useConversationStore();
	const { addConversation } = conversationStore;
	const { conversations, isFetching } = storeToRefs(conversationStore);
	const { userInfo } = storeToRefs(useUserStore());
	const errorMessage = ref("");

	const { handleSubmit, errors } = useForm({
		validationSchema: startConversationSchema
	});

	const { value: userId } = useField("userId");

	const {
		isFetching: isFetchingPost,
		statusCode,
		response,
		data,
		execute
	} = useFetch<Conversation>(
		"/api/conversations",
		{
			credentials: "include"
		},
		{
			immediate: false
		}
	)
		.post(() => ({ userId: userId.value }))
		.json<Conversation>()

	const startConversation = handleSubmit(async () => {
		await execute(false);

		if (statusCode.value !== 201)
			errorMessage.value = await response.value?.json();

		if (data.value) {
			getSignalR()?.invoke("JoinConversation", data.value?.id);
			addConversation(data.value);
		}
	});

	const getDate = (dateStr: string) => {
		const date = new Date(dateStr)
		//TODO add today and yesterday check
		return new Intl.DateTimeFormat("en-GB", {
			weekday: "long",
			day: "2-digit",
			month: "short",
			year: "numeric"
		}).format(date)
	}

	const getTime = (dateStr: string) => {
		const date = new Date(dateStr);

		return new Intl.DateTimeFormat("en-GB", {
			hour: "2-digit",
			minute: "2-digit",
			hour12: false
		}).format(date);
	}
</script>

<template>
	<div class="conversation-list">
		<div v-if="isFetching">Loading...</div>
		<div v-else>
			<div v-for="c in conversations">
				<RouterLink :to="{name: 'conversations', params: { id: c.id}}">
					<span v-if="c.type === 'Direct'">{{
						c.members.find((cm) => cm.userId !== userInfo?.id)?.user.userName
					}}</span>
					<span v-if="c.type === 'Group'">{{ c.name }}</span>
					<span v-if="c.lastMessageSnippet">{{ c.lastMessageSnippet }}</span>
					<span v-if="c.lastMessageSentAt">{{ getDate(c.lastMessageSentAt) }}</span>
					<span v-if="c.lastMessageSentAt">{{ getTime(c.lastMessageSentAt) }}</span>
				</RouterLink>
				<hr />
			</div>
			<div v-if="isFetchingPost">Loading...</div>
			<form @submit="startConversation" v-else>
				Start a conversation!
				<label>
					User ID:
					<input name="userId" v-model="userId" type="text" />
				</label>
				<button>Find</button>
				<span v-if="errors.userId">{{ errors.userId }}</span>
				<span v-if="errorMessage">{{ errorMessage }}</span>
			</form>
		</div>
	</div>
</template>

<style scoped>
	@reference "../style.css";

	.conversation-list {
		@apply flex flex-col w-1/3 h-svh bg-blue-500;
	}
</style>
