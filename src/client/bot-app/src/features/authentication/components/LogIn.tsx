import useSmallScreen from "../../../hooks/hooks";





export const Login = () => {
const isSmallScreen = useSmallScreen();
console.log(`Logging in. Is Mbolie = ${isSmallScreen}`)

return (
<form onSubmit={() => console.log('Submitting login details')}
>

</form>
)

}