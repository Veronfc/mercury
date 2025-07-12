const registerSchema = toTypedSchema(
	zod.object({
		email: zod.string().email("Invalid email address"),
		password: zod
			.string()
			.regex(
				/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/,
				"Password must be at least 8 characters long and include:\nAt least one digit\nAt least one lower case alphabet\nAt least one upper case alphabet\nAt least one special character (@, $, !, %, *, ?, &)"
			)
	})
);

const loginSchema = toTypedSchema(
	zod.object({
		email: zod.string().email("Invalid email address"),
		password: zod.string() //additional validation?
	})
);

export { registerSchema, loginSchema };
