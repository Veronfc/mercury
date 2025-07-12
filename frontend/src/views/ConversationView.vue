<script setup lang="ts">
	const { isLoggedIn } = storeToRefs(useUserStore());
	const router = useRouter();

	if (!isLoggedIn.value) {
		router.replace({ name: "auth" });
	}
</script>

<template>
	<div>
		<ConversationList />
		<MessageList />
	</div>
</template>

<style scoped>
	div {
		@apply flex h-svh w-svw;
	}
</style>
