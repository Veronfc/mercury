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
	avatarUrl: string;
	lastActive: string;
};

type Message = {
	id: string;
	conversationId: string;
	senderId: string;
	content: string;
	sentAt: string;
};

type Friend = {
	friendId: string;
	friend: User;
	friendSince: string; //DateTime 
}

type FriendRequestSent = {
	receiverId: string;
	receiver: User;
	sentAt: string; //DateTime
	status: FriendRequestStatus;
}

type FriendRequestReceived = {
	requesterId: string;
	requester: User;
	sentAt: string; //DateTime
	status: FriendRequestStatus;
}

type FriendRequestStatus = "Pending" | "Rejected" | "Accepted";

export type {
	Conversation,
	ConversationType,
	ConversationMember,
	User,
	Message,
	Friend,
	FriendRequestSent,
	FriendRequestReceived,
	FriendRequestStatus
};
