<script setup lang="ts">
	import { Icon } from "@iconify/vue";

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
		"/api/conversations/direct",
		{
			credentials: "include"
		},
		{
			immediate: false
		}
	)
		.post(() => ({ userId: userId.value }))
		.json<Conversation>();

	const startConversation = handleSubmit(async () => {
		await execute(false);

		if (statusCode.value !== 201)
			errorMessage.value = await response.value?.json();

		if (data.value) {
			await getSignalR()?.invoke("JoinConversation", data.value?.id);

			addConversation(data.value);
		}

		userId.value = "";
	});

	const getDate = (dateStr: string | undefined) => {
		if (!dateStr) return;

		const date = new Date(dateStr);
		//TODO add today and yesterday check
		return new Intl.DateTimeFormat("en-GB", {
			weekday: "long",
			day: "2-digit",
			month: "short",
			year: "numeric"
		}).format(date);
	};

	const getTime = (dateStr: string | undefined) => {
		if (!dateStr) return;

		const date = new Date(dateStr);

		return new Intl.DateTimeFormat("en-GB", {
			hour: "2-digit",
			minute: "2-digit",
			hour12: false
		}).format(date);
	};
</script>

<template>
	<div class="conversation-list">
		<div v-if="isFetching">Loading...</div>
		<div v-else v-for="c in conversations" class="conversation">
			<RouterLink :to="{ name: 'conversations', params: { id: c.id } }">
				<span v-if="c.type === 'Direct'"
					>{{
						c.members.find((cm) => cm.userId !== userInfo?.id)?.user.displayName
							? c.members.find((cm) => cm.userId !== userInfo?.id)?.user
									.displayName
							: c.members.find((cm) => cm.userId !== userInfo?.id)?.user
									.userName
					}}Last seen:
					{{
						getDate(
							c.members.find((cm) => cm.userId !== userInfo?.id)?.user
								.lastActive
						)
					}}
					-
					{{
						getTime(
							c.members.find((cm) => cm.userId !== userInfo?.id)?.user
								.lastActive
						)
					}}</span
				>
				<span v-if="c.type === 'Group'">{{ c.name }}</span>
				<span v-if="c.lastMessageSnippet">{{ c.lastMessageSnippet }}</span>
				<Icon
					:icon="
						c.lastMessageSenderId === userInfo?.id
							? 'mdi:call-made'
							: 'mdi:call-received'
					" />
				<div class="date-time">
					<span v-if="c.lastMessageSentAt">{{
						getDate(c.lastMessageSentAt)
					}}</span>
					<span v-if="c.lastMessageSentAt">{{
						getTime(c.lastMessageSentAt)
					}}</span>
				</div>
			</RouterLink>
		</div>
		<div v-if="isFetchingPost">Loading...</div>
		<form v-else @submit="startConversation" class="conversation-start">
			Start a conversation!
			<label>
				User ID:
				<input name="userId" v-model="userId" type="text" />
			</label>
			<button><Icon icon="material-symbols:person-search" /></button>
			<span v-if="errors.userId">{{ errors.userId }}</span>
			<span v-if="errorMessage">{{ errorMessage }}</span>
		</form>
	</div>
</template>

<style scoped>
	@reference "../style.css";

	.conversation-list {
		@apply flex flex-col w-max h-svh bg-dark text-white p-4 gap-4 overflow-y-scroll overflow-x-auto;

		.conversation {
			@apply border border-medium flex flex-col rounded bg-medium text-light p-2;

			.date-time {
				@apply flex content-between text-xs;
			}
		}

		.conversation-start {
			@apply bg-main text-light rounded p-2 flex flex-col items-center gap-2;

			input {
				@apply border rounded p-1;
			}

			button {
				@apply cursor-pointer bg-action rounded text-light w-min p-1;

				svg {
					@apply text-2xl;
				}
			}
		}
	}
</style>
