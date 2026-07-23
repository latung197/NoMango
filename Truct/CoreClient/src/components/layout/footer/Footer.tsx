import React from 'react'
import { useTranslation } from 'react-i18next'

// import localtionIcon from '@/assets/icons/location.png';
// import phoneIcon from '@/assets/icons/phone-call.png';
// import earthGlobeIcon from '@/assets/icons/earth-globe.png';

const Footer: React.FC = () => {
  const { t } = useTranslation()
  const year = new Date().getFullYear()

  return (
    <div>
      <footer className="mt-12 hidden w-full bg-[#F8F8F8] px-4 py-8 text-black md:grid"></footer>
    </div>
  )
}

export default Footer
