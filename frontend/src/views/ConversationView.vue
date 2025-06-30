<script setup lang="ts">
	import ConversationList from "../components/ConversationList.vue";
	import MessageList from "../components/MessageList.vue";
	import { useUserStore } from "../stores/userStore";
	import { useRouter } from "vue-router";
	//TODO add default local.dev/conversation check to MessageList component to show an empty component
	const { isLoggedIn } = useUserStore();
	const router = useRouter();

	if (!isLoggedIn) {
		router.replace({ name: "auth" });
	}
</script>

<template>
	<ConversationList />
	<MessageList />
	<div></div>
</template>

<style scoped></style>
