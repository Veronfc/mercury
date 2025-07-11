import { toTypedSchema } from "@vee-validate/zod";
import * as zod from "zod";

const sendMessageSchema = toTypedSchema(
	zod.object({
		content: zod
			.string()
			.nonempty("Message content can not be empty")
			.max(2500, "Message content must be at most 2500 characters long")
	})
);

export { sendMessageSchema };
