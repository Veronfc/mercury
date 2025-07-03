<script setup lang="ts">
	import { storeToRefs } from "pinia";
	import { useMessageStore } from "../stores/messageStore";
	import { useRoute } from "vue-router";
	import { computed, watch } from "vue";
	import { useConversationStore } from "../stores/conversationStore";

	const route = useRoute();
	const { conversations } = storeToRefs(useConversationStore());
	const messageStore = useMessageStore();
	const { getMessages } = messageStore;
	const { isFetching, messages } = storeToRefs(messageStore);

	const conversationId = computed(() => route.params.id as string);

	watch(
		conversationId,
		(id) => {
			if (!conversations.value[id]) {
				return;
			}

			getMessages(id);
		},
		{ immediate: true }
	);
</script>

<template>
	<div v-if="isFetching"></div>
	<div v-else>
		<div v-for="m in messages[conversationId]">
			<span>{{ m.content }}</span>
			<span>{{ m.senderId }}</span>
			<span>{{ m.sentAt }}</span>
		</div>
	</div>
</template>

<style scoped></style>
