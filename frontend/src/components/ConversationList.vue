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
		"/api/conversation",
		{
			credentials: "include"
		},
		{
			immediate: false
		}
	)
		.post({ userId: userId.value })
		.json<Conversation>();

	const startConversation = handleSubmit(async () => {
		await execute;

		if (statusCode.value !== 201)
			errorMessage.value = await response.value?.json();

		if (data.value) {
			getSignalR()?.invoke("JoinConversation", data.value?.id);
			addConversation(data.value);
		}
	});
</script>

<template>
	<div v-if="isFetching">Loading...</div>
	<div v-else>
		<div v-for="c in conversations">
			<span v-if="c.type === 'Direct'">{{
				c.members.find((cm) => cm.userId !== userInfo?.id)?.user.userName
			}}</span>
			<span v-if="c.type === 'Group'">{{ c.name }}</span>
			<span>{{ c.lastMessageSnippet }}</span>
			<span>{{ c.lastMessageSentAt }}</span>
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
</template>

<style scoped></style>
