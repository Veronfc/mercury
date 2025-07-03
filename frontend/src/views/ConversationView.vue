<script setup lang="ts">
	import { storeToRefs } from "pinia";
	import ConversationList from "../components/ConversationList.vue";
	import MessageList from "../components/MessageList.vue";
	import { useUserStore } from "../stores/userStore";
	import { useRouter } from "vue-router";
	const { isLoggedIn } = storeToRefs(useUserStore());
	const router = useRouter();

	if (!isLoggedIn.value) {
		router.replace({ name: "auth" });
	}
</script>

<template>
	<ConversationList />
	<MessageList />
	<div></div>
</template>

<style scoped></style>
