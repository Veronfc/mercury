<script setup lang="ts">
	import { useConversationStore } from "../stores/useConversationStore";
	import { useUserStore } from "../stores/useUserStore";
	//add stuff like in the message store
	const conversationStore = useConversationStore();
	const userStore = useUserStore();
</script>

<template>
	<div>
		<div v-for="c in conversationStore.conversations">
			<span v-if="c.type === 'Direct'">{{
				c.members.find((cm) => cm.userId !== userStore.userInfo?.id)?.user
					.userName
			}}</span>
			<span v-if="c.type === 'Group'">{{ c.name }}</span>
			<span>{{ c.lastMessageSnippet }}</span>
			<span>{{ c.lastMessageSentAt }}</span>
			<span>{{ c. }}</span>
			<hr />
		</div>
	</div>
</template>

<style scoped></style>
