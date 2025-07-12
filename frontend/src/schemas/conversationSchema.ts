const startConversationSchema = toTypedSchema(
	zod.object({
		userId: zod.string().nonempty("User ID must not be empty")
	})
);

export { startConversationSchema };
