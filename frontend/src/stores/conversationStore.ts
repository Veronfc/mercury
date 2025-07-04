import { defineStore } from "pinia";
import { ref } from "vue";
import type { Conversation } from "../types";
import { useFetch } from "@vueuse/core";

export const useConversationStore = defineStore(
	"conversations",
	() => {
		const conversations = ref<Conversation[]>([]);
		const isFetching = ref(false);

		const sortConversations = () => {
			conversations.value.sort((a, b) => {
				const sentAtA = a.lastMessageSentAt ? new Date(a.lastMessageSentAt).getTime() : 0;
				const sentAtB = b.lastMessageSentAt ? new Date(b.lastMessageSentAt).getTime() : 0;

				return sentAtB - sentAtA;
			})
		}

		const getConversation = (id: string) => {
			return conversations.value.find(c => c.id === id);
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
				conversations.value = data.value;
				sortConversations();
			}
		};

		const addConversation = (conversation: Conversation) => {
			conversations.value.push(conversation);
		};

		const updateConversation = (updatedConversation: Conversation) => {
			const index = conversations.value.findIndex(c => c.id === updatedConversation.id)
			conversations.value[index] = updatedConversation;
			sortConversations();
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
