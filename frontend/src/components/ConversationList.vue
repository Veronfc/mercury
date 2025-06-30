<script setup lang="ts">
	import { storeToRefs } from "pinia";
	import { useConversationStore } from "../stores/conversationStore";
	import { useUserStore } from "../stores/userStore";
	const { conversations, isFetching } = storeToRefs(useConversationStore());
	const { userInfo } = storeToRefs(useUserStore());
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
	</div>
</template>

<style scoped></style>
