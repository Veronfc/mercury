import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection | null = null;

const connectSignalR = async () => {
	const { addMessage } = useMessageStore();
	const { getConversation, updateConversation } = useConversationStore();
	const { userInfo } = storeToRefs(useUserStore());

	if (connection) return connection;

	connection = new signalR.HubConnectionBuilder()
		.withUrl("/api/hub", { withCredentials: true })
		.withAutomaticReconnect()
		.build();

	connection.on("ReceiveMessage", (message: Message) => {
		addMessage(message);

		const conversation = getConversation(message.conversationId);
		conversation!.lastMessageSentAt = message.sentAt;
		conversation!.lastMessageSnippet = message.content.substring(0, 100);
		conversation!.lastMessageSenderId = userInfo.value?.id;

		updateConversation(conversation!);
	});

	await connection.start();
	console.log("Connected to hub");
	return connection;
};

const getSignalR = () => connection;

const disconnectSignalR = async () => {
	if (connection) {
		await connection.stop();
		console.log("Disconnected from hub");
		connection = null;
	}
};

export { connectSignalR, disconnectSignalR, getSignalR };
