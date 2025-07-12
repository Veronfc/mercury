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

	// FIXME validate only onsubmit
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

			content.value = "";
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
		<TransitionGroup name="toast">
			<div v-if="errors.content" class="toast error">
				{{ errors.content }}
			</div>
			<div v-if="isSubmitting" class="toast progress">Sending</div>
		</TransitionGroup>
	</div>
	<div v-else class="message-list"></div>
</template>

<style scoped>
	@reference "../style.css";

	.message-list {
		@apply flex h-svh w-full flex-col gap-4 bg-gradient-to-br from-primary-bgd via-secondary-bgd to-tertiary-bgd p-4;

		/* FIXME message left and right alignment */
		.messages {
			@apply flex h-full w-full flex-col-reverse items-start gap-2 overflow-y-scroll;

			.message {
				@apply flex w-max flex-col rounded border border-tertiary-bgd p-2 bg-tertiary-bgd text-primary-txt duration-150;

				&:hover {
					@apply border-secondary-act;
				}
			}

			.right {
				@apply bg-primary-bgd text-secondary-txt border-primary-bgd self-end;

				&:hover {
					@apply border-primary-act;
				}
			}
		}

		.message-new {
			@apply w-full;

			form {
				@apply flex gap-2;

				input {
					@apply w-full rounded border border-primary-bgd bg-primary-bgd py-2 px-3 text-primary-txt outline-0 duration-300;

					&:focus, &:hover {
						@apply border-primary-act;
					}

					&::placeholder {
						@apply text-secondary-txt;
					}
				}

				button {
					@apply cursor-pointer rounded bg-primary-act p-2;

					svg {
						@apply text-2xl text-primary-txt;
					}
				}
			}
		}
	}
</style>
