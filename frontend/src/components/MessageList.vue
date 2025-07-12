<script setup lang="ts">
	import { Icon } from "@iconify/vue";

	const route = useRoute();
	const { getConversation } = useConversationStore();
	const messageStore = useMessageStore();
	const { getMessages } = messageStore;
	const { isFetching, messages } = storeToRefs(messageStore);
	const { userInfo } = storeToRefs(useUserStore());

	const conversationId = computed(() => route.params.id as string);

	const getDate = (dateStr: string) => {
		const date = new Date(dateStr);
		//TODO add today and yesterday check
		return new Intl.DateTimeFormat("en-GB", {
			weekday: "short",
			day: "2-digit",
			month: "short",
			year: "numeric"
		}).format(date);
	};

	const getTime = (dateStr: string) => {
		const date = new Date(dateStr);

		return new Intl.DateTimeFormat("en-GB", {
			hour: "2-digit",
			minute: "2-digit",
			hour12: false
		}).format(date);
	};

	const { handleSubmit, errors, isSubmitting } = useForm({
		validationSchema: sendMessageSchema
	});

	const { value: content } = useField<string>("content");

	const sendMessage = handleSubmit(async (values) => {
		await getSignalR()?.invoke(
			"SendMessage",
			conversationId.value,
			values.content
		);

		content.value = "";
	});

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
	<div class="message-list" v-if="getConversation(conversationId)">
		<div v-if="isFetching" class="messages">Loading...</div>
		<div class="messages" v-else>
			<div v-for="m in messages[conversationId]">
				<div
					class="message"
					:class="m.senderId === userInfo?.id ? 'right' : ''">
					<span>{{ m.content }}</span>
					<span>{{ getDate(m.sentAt) }}</span>
					<span>{{ getTime(m.sentAt) }}</span>
				</div>
			</div>
		</div>
		<div class="message-new">
			<form @submit="sendMessage">
				<input
					name="content"
					v-model="content"
					type="text"
					placeholder="Type a message..." />
				<button><Icon icon="ri:send-plane-fill" /></button>
			</form>
		</div>
		<div class="message-error" v-if="errors.content">
			{{ errors.content }}
		</div>
		<div class="message-submission" v-if="isSubmitting">Sending...</div>
	</div>
	<div v-else class="message-list"></div>
</template>

<style scoped>
	@reference "../style.css";

	.message-list {
		@apply flex flex-col h-svh w-full p-4 gap-4 bg-main relative;

		.messages {
			@apply flex flex-col-reverse gap-2 items-start w-full h-full overflow-y-scroll;

			.message {
				@apply flex flex-col border rounded w-max p-2;
			}

			.right {
				@apply bg-black text-white;
			}
		}

		.message-new {
			@apply w-full;

			form {
				@apply flex gap-2;

				input {
					@apply border rounded w-full p-2 bg-black text-white border-black;
				}

				button {
					@apply bg-black rounded p-2 cursor-pointer;

					svg {
						@apply text-2xl text-white;
					}
				}
			}
		}

		.message-error {
			@apply absolute top-4 right-4 border-2 border-red-500 rounded p-2;
		}

		.message-submission {
			@apply absolute top-4 right-4 border-2 border-green-500 rounded p-2;
		}
	}
</style>
