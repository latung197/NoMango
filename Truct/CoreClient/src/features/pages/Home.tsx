import React from 'react'
import { useState, useEffect, useRef } from 'react'
import { useTranslation } from 'react-i18next'
import 'swiper/css'
import { ProductByCategoryPriorityResponse } from '@/types/Product'
import { getProductGroupByCategory } from '@/services/product.service'
import { ApiResponse } from '@/types/ApiResponse'
import { House } from 'lucide-react'

const Home: React.FC = () => {
  const { t, i18n } = useTranslation()
  const [loading, setLoading] = useState(true)
  const asideRef = useRef<HTMLDivElement>(null)
  const [asideHeight, setAsideHeight] = useState<number>(0)
  const [asideTop, setAsideTop] = useState<number>(0)

  useEffect(() => {
    if (asideRef.current) {
      const rect = asideRef.current.getBoundingClientRect()
      setAsideHeight(rect.height)
      setAsideTop(rect.top)
    }
  }, [])

  const [productGroups, setProductGroups] =
    useState<ApiResponse<ProductByCategoryPriorityResponse[]>>()
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        setLoading(true)
        const data = await getProductGroupByCategory()
        setProductGroups(data)
      } finally {
        setLoading(false)
      }
    }
    fetchCategories()
  }, [i18n.language])

  return (
    <>
      <main className="flex flex-col items-center bg-white"></main>
    </>
  )
}

export default Home
