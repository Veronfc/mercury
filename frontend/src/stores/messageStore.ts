export const useMessageStore = defineStore(
	"messages",
	() => {
		const messages = ref<Record<string, Message[]>>({});
		const isFetching = ref(false);

		const getMessages = async (conversationId: string) => {
			//if (messages.value[conversationId]) return;

			isFetching.value = true;

			const { statusCode, data } = await useFetch<Message[]>(
				`/api/messages?conversationId=${conversationId}`
			)
				.get()
				.json<Message[]>();

			isFetching.value = false;

			if (statusCode.value === 200 && data.value) {
				messages.value[conversationId] = data.value;
				return;
			}

			console.log(statusCode.value);
		};

		const addMessage = (message: Message) => {
			messages.value[message.conversationId].unshift(message);
		};

		return { messages, isFetching, getMessages, addMessage };
	},
	{
		persist: {
			storage: sessionStorage
		}
	}
);
