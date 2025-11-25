import LoginForm from './LoginForm';
import bannerLoginImg from '@/assets/images/Banner_login.png';
import { getToken } from '@/utils/authUtils';

const LoginPage = () => {
    
    const token = getToken();
    if (token) {
        window.location.href = "/";
    }

    return (
        <div className="flex justify-center items-center py-10">
            <div className="w-full max-w-screen-xl flex flex-col lg:flex-row overflow-hidden">
                <div className="lg:w-1/2 w-full p-6">
                    <img
                        src={bannerLoginImg}
                        alt="Login"
                        className="w-full h-full object-cover rounded-md"
                    />
                </div>
                <div className="lg:w-1/2 w-full">
                    <LoginForm />
                </div>
            </div>
        </div>
    );
};

export default LoginPage;