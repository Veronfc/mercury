import { defineStore } from "pinia";
import { ref } from "vue";
import type { Conversation } from "../types";
import { useFetch } from "@vueuse/core";

export const useConversationStore = defineStore(
	"conversations",
	() => {
		const conversations = ref<Record<string, Conversation>>({});
		const isFetching = ref(false);

		const getConversation = (id: string) => {
			return conversations.value[id];
		};

		const getConversations = async () => {
			isFetching.value = true;

			const { statusCode, data } = await useFetch<Conversation[]>(
				"/api/conversations",
				{
					credentials: "include"
				}
			)
				.get()
				.json<Conversation[]>();

			isFetching.value = false;

			if (statusCode.value === 200 && data.value) {
				conversations.value = Object.fromEntries(
					data.value?.map((c) => [c.id, c])
				);
			}
		};

		const addConversation = (conversation: Conversation) => {
			conversations.value[conversation.id] = conversation;
		};

		const updateConversation = (updatedConversation: Conversation) => {
			conversations.value[updatedConversation.id] = updatedConversation;
		};

		return {
			conversations,
			isFetching,
			getConversation,
			getConversations,
			addConversation,
			updateConversation
		};
	},
	{
		persist: {
			storage: sessionStorage
		}
	}
);
