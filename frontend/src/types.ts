type Conversation = {
	id: string;
	type: ConversationType;
	name?: string;
	lastMessageSentAt?: string; //DateTime
	lastMessageSnippet?: string;
	lastMessageSenderId?: string;
	members: ConversationMember[];
};

type ConversationType = "Direct" | "Group";

type ConversationMember = {
	userId: string;
	user: User;
};

type User = {
	id: string;
	email: string;
	userName: string;
	displayName: string;
	lastActive: string;
};

type Message = {
	id: string;
	conversationId: string;
	senderId: string;
	content: string;
	sentAt: string;
};

export type {
	Conversation,
	ConversationType,
	ConversationMember,
	User,
	Message
};
