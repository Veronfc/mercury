<script setup lang="ts">
	import { Icon } from "@iconify/vue";

	const { userInfo, isLoggedIn } = storeToRefs(useUserStore());
	const dismiss = ref(false);
</script>

<template>
	<div
		v-if="isLoggedIn && !userInfo?.displayName && !dismiss"
		class="notification">
		<span>You don't have a display name</span>
		<RouterLink @click="dismiss = !dismiss" :to="{ name: 'user' }" class="link"
			>Set it now</RouterLink
		>
		<button @click="dismiss = !dismiss">
			<Icon icon="fluent:dismiss-12-filled" />
		</button>
	</div>
	<Suspense>
		<RouterView />
	</Suspense>
</template>

<style scoped>
	@reference './style.css';

	.notification {
		@apply absolute top-4 left-1/2 flex -translate-x-1/2 flex-col items-center gap-2 rounded border-2 border-red-500 bg-white p-2;

		.link {
			@apply border p-2;
		}

		button {
			@apply cursor-pointer;
		}
	}
</style>
