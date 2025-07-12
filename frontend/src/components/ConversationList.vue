<script setup lang="ts">
	import { Icon } from "@iconify/vue";

	const conversationStore = useConversationStore();
	const { addConversation } = conversationStore;
	const { conversations, isFetching } = storeToRefs(conversationStore);
	const { userInfo } = storeToRefs(useUserStore());
	const errorMessage = ref("");
	const startConversationOpen = ref(true);

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
			errorMessage.value = await response.value?.text()!;

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
		<RouterLink
			v-else
			v-for="c in conversations"
			:to="{ name: 'conversations', params: { id: c.id } }"
			class="conversation"
			exact-active-class="conversation-active">
			<span v-if="c.type === 'Direct'"
				>{{
					c.members.find((cm) => cm.userId !== userInfo?.id)?.user.displayName
						? c.members.find((cm) => cm.userId !== userInfo?.id)?.user
								.displayName
						: c.members.find((cm) => cm.userId !== userInfo?.id)?.user.userName
				}}Last seen:
				{{
					getDate(
						c.members.find((cm) => cm.userId !== userInfo?.id)?.user.lastActive
					)
				}}
				-
				{{
					getTime(
						c.members.find((cm) => cm.userId !== userInfo?.id)?.user.lastActive
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
		<TransitionGroup name="start-conversation">
			<form v-if="startConversationOpen" @submit="startConversation">
				<div v-if="isFetchingPost">Loading...</div>
				<div v-else class="start-conversation">
					Find user by ID
					<input
						name="userId"
						v-model="userId"
						type="text"
						placeholder="Enter User ID here" />
					<button>Search</button>
					<span v-if="errors.userId">{{ errors.userId }}</span>
					<span v-if="errorMessage">{{ errorMessage }}</span>
					<Icon @click="startConversationOpen = !startConversationOpen" icon="material-symbols:cancel-presentation-rounded" class="start-conversation-close"/>
				</div>
			</form>
			<button v-else @click="startConversationOpen = !startConversationOpen" class="start-conversation-open">New conversation</button>
		</TransitionGroup>
	</div>
</template>

<style scoped>
	@reference "../style.css";

	.conversation-list {
		@apply flex h-svh w-max flex-col gap-4 overflow-x-auto overflow-y-scroll bg-primary-bgd p-4 text-primary-txt;

		.conversation {
			@apply flex flex-col rounded border border-tertiary-bgd bg-tertiary-bgd p-2 text-primary-txt duration-300;

			&:hover {
				@apply border-primary-act;
			}

			.date-time {
				@apply flex content-between text-xs;
			}
		}

		.conversation-active {
			@apply border-primary-act bg-primary-act;
		}

		.start-conversation {
			@apply flex flex-col relative items-start gap-4 rounded border border-secondary-bgd bg-secondary-bgd p-4 text-primary-txt;

			&:hover {
				@apply border-secondary-act;
			}

			input {
				@apply w-full rounded border border-secondary-txt px-3 py-2 outline-0 outline-secondary-act;

				&::placeholder {
					@apply text-secondary-txt;
				}
			}

			button {
				@apply cursor-pointer rounded border border-secondary-act bg-secondary-act py-2 px-4 w-full text-primary-bgd duration-300;
			}

			.start-conversation-close {
				@apply text-secondary-act text-4xl absolute top-3 right-3 cursor-pointer;
			}
		}

		.start-conversation-open {
			@apply cursor-pointer rounded border border-secondary-act bg-secondary-act py-2 px-4 w-full text-primary-bgd;
		}
	}

	.start-conversation-enter-active,
.start-conversation-leave-active {
	transition: all 0.5s ease;
}
.start-conversation-enter-from {
	opacity: 0;
	transform: translateY(1rem);
}

.start-conversation-leave-to {
	opacity: 0;
	transform: translateY(-1rem);
}
</style>
