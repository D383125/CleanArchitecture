import useSmallScreen from "../../../hooks/hooks";


export const SignUp = () => {
const isSmallScreen = useSmallScreen();
console.log(`Signing user up. Is Mbolie = ${isSmallScreen}`)

return (
<form
    onSubmit={() => console.log('Submitting Sign up data')}
></form>
)
}