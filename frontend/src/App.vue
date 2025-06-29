<script setup lang="ts">
	import { useUserStore } from "./stores/useUserStore";

	const userStore = useUserStore();

	if (!userStore.isLoggedIn) {
		await userStore.setLoggedIn();
		await userStore.setInfo();
	}
</script>

<template>
	<Suspense>
		<RouterView />
	</Suspense>
</template>

<style scoped></style>
