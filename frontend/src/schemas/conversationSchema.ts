import { toTypedSchema } from "@vee-validate/zod";
import * as zod from "zod";

const startConversationSchema = toTypedSchema(
	zod.object({
		userId: zod.string().nonempty("User ID must not be empty")
	})
);

export { startConversationSchema };
