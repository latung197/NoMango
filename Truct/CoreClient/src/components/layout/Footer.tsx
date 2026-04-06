import React from 'react';
import { useTranslation } from 'react-i18next';

// import localtionIcon from '@/assets/icons/location.png';
// import phoneIcon from '@/assets/icons/phone-call.png';
// import earthGlobeIcon from '@/assets/icons/earth-globe.png';

const Footer: React.FC = () => {
    const { t } = useTranslation();
    const year = new Date().getFullYear();
    const phoneNumber = t('footer.phone');

    return (
        <div>
            <footer className="hidden md:grid w-full bg-[#F8F8F8] text-black py-8 px-4 mt-12">
                <div className="max-w-screen-xl mx-auto grid grid-cols-1 md:grid-cols-3 gap-8 px-4">
                    <div className="flex justify-center items-center">
                        <img src="https://fstv.vn/wp-content/themes/fstv/img/common/header_logo.png" alt="FA SYSTEM Logo" className="h-30 rounded-sm" />
                    </div>

                    <div className="flex flex-col justify-between h-full">
                    <div className="flex-1" />
                    <p className="text-xs text-[#717171] text-center">
                        Copyright {year} © FA SYSTEM & TECHNOLOGY VIETNAM CO., LTD.
                    </p>
                    </div>

                    <div className="text-black text-left">
                        <h4 className="font-semibold mb-2 text-base">{t('footer.contact')}</h4>
                        <ul className="text-sm space-y-1 leading-relaxed">
                            <li className="flex items-center space-x-2">
                                {/* <img src={localtionIcon} alt={t('footer.location')} className="w-5 h-5" /> */}
                                <span className="whitespace-nowrap md:whitespace-nowrap" >{t('footer.address')}</span>
                            </li>
                            <li className="flex items-center space-x-2">
                                {/* <img src={phoneIcon} alt={t('footer.phone')} className="w-5 h-5" /> */}
                                <a href={`tel:${phoneNumber}`} className="text-[#009EE2] hover:underline">
                                    {t('footer.phone')}
                                </a>
                            </li>
                            <li className="flex items-center space-x-2">
                                {/* <img src={earthGlobeIcon} alt={t('footer.website')} className="w-5 h-5" /> */}
                                <a
                                    href="https://fstv.vn"
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="underline text-[#009EE2] hover:underline"
                                >
                                    {t('footer.website')}
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>
            </footer>
            <footer className="block md:hidden bg-gray-100 p-4 text-center">
                <div className="max-w-screen-xl mx-auto grid grid-cols-1 md:grid-cols-3 gap-8 px-2 text-center md:text-left">
                    {/* Contact */}
                    <div className='text-left'>
                        <h4 className="font-semibold mb-2 text-base">{t('footer.contact')}</h4>
                        <ul className="text-sm space-y-2 leading-relaxed">
                            <li className="flex items-center justify-start space-x-2">
                            {/* <img src={localtionIcon} alt={t('footer.location')} className="w-5 h-5" /> */}
                            <span>{t('footer.address')}</span>
                            </li>
                            <li className="flex items-center justify-start space-x-2">
                            {/* <img src={phoneIcon} alt={t('footer.phone')} className="w-5 h-5" /> */}
                            <a href={`tel:${phoneNumber}`} className="text-[#009EE2] hover:underline">
                                {t('footer.phone')}
                            </a>
                            </li>
                            <li className="flex items-center justify-start space-x-2">
                            {/* <img src={earthGlobeIcon} alt={t('footer.website')} className="w-5 h-5" /> */}
                            <a
                                href="https://fstv.vn"
                                target="_blank"
                                rel="noopener noreferrer"
                                className="text-[#009EE2] underline hover:underline"
                            >
                                {t('footer.website')}
                            </a>
                            </li>
                        </ul>
                    </div>
                    {/* Copyright */}
                    <div className="flex flex-col justify-between items-center md:items-start">
                        <div className="flex-1" />
                        <p className="text-xs text-[#717171] md:mt-0">
                            © {year} FA SYSTEM & TECHNOLOGY VIETNAM CO., LTD.
                        </p>
                    </div>

                </div>
            </footer>
        </div>

  );
};

export default Footer;