const setDisplayNameSchema = toTypedSchema(
	zod.object({
		displayName: zod
			.string()
			.min(5, "Display name must be at least 5 characters long")
			.max(30, "Display name must be at most 30 characters long")
	})
);

export { setDisplayNameSchema };
