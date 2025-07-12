<script setup lang="ts">
	const { userInfo } = storeToRefs(useUserStore());

	const { handleSubmit, errors } = useForm({
		validationSchema: setDisplayNameSchema
	});
	const { value: displayName } = useField(
		"displayName",
		{},
		{ initialValue: userInfo.value?.displayName }
	);

	const { isFetching, statusCode, response, execute } = useFetch(
		"/api/user/displayname",
		{
			credentials: "include"
		},
		{
			immediate: false
		}
	).post(() => ({
		displayName: displayName.value
	}));

	const successMessage = ref("");
	const errorMessage = ref("");
	const { setInfo } = useUserStore();

	const setDisplayName = handleSubmit(async () => {
		await execute(false);

		if (statusCode.value !== 200) {
			errorMessage.value = await response.value?.text()!;

			setTimeout(async () => {
				errorMessage.value = "";
			}, 1500);

			return;
		}

		if (statusCode.value === 200) {
			successMessage.value = await response.value?.text()!;

			setInfo();
		}
	});
</script>

<template>
	<div v-if="isFetching">Loading...</div>
	<div v-else>
		<form @submit="setDisplayName">
			<label>
				Display name:
				<input name="displayname" v-model="displayName" type="text" />
			</label>
			<button>Save</button>
		</form>
		<TransitionGroup name="toast" tag="div">
			<span v-if="successMessage" class="toast success">{{
				successMessage
			}}</span>
			<span v-if="errors.displayName" class="toast error">{{
				errors.displayName
			}}</span>
			<span v-if="errorMessage" class="toast error">{{ errorMessage }}</span>
		</TransitionGroup>
	</div>
</template>

<style scoped>
	@reference "../style.css";
</style>
