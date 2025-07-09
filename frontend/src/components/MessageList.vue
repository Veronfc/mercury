<script setup lang="ts">
	import { computed, ref, watch } from "vue";
	import { useRoute } from "vue-router";
	import { storeToRefs } from "pinia";
	import {Icon} from "@iconify/vue"
	import { useMessageStore } from "../stores/messageStore";
	import { useConversationStore } from "../stores/conversationStore";
import { getSignalR } from "../lib/hub";
	
	const route = useRoute();
	const { getConversation } = useConversationStore();
	const messageStore = useMessageStore();
	const { getMessages } = messageStore;
	const { isFetching, messages } = storeToRefs(messageStore);

	const conversationId = computed(() => route.params.id as string);

	const getDate = (dateStr: string) => {
		const date = new Date(dateStr)
		//TODO add today and yesterday check
		return new Intl.DateTimeFormat("en-GB", {
			weekday: "short",
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

	//TODO add vee-validate/zod form validation
	const connection = getSignalR();
	const message = ref("");

	const sendMessage = async () => {
		await connection?.invoke("SendMessage", conversationId.value, message.value);

		message.value = ""
	}

	watch(
		conversationId,
		(id) => {
			if (!getConversation(id)) {
				return;
			}

			getMessages(id);
		},
		{ immediate: true }
	);
</script>

<template>
	<div class="message-list">
		<div v-if="isFetching">Loading...</div>
		<div class="messages" v-else>
			<div v-for="m in messages[conversationId]">
				<span>{{ m.content }}</span>
				<span>{{ m.senderId }}</span>
				<span>{{ getDate(m.sentAt) }}</span>
				<span>{{ getTime(m.sentAt) }}</span>
				<hr>
			</div>
		</div>
		<div class="message-new">
			<form @submit.prevent="sendMessage">
				<input v-model="message">
				<button><Icon icon="ri:send-plane-fill"/></button>
			</form>
		</div>
	</div>
</template>

<style scoped>
	@reference "../style.css";

	.message-list {
		@apply flex flex-col relative h-svh bg-amber-500;

		.messages {
			@apply flex flex-col;
		}

		.message-new {
			@apply p-4;

			form {
				@apply flex bottom-4 absolute;

				input {
					@apply border rounded;
				}
	
				svg {
					@apply text-2xl bg-black text-white rounded cursor-pointer;
				}
			}
		}
	}
</style>
