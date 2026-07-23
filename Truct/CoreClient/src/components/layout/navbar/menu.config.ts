export interface MenuItem {
  id: string
  label: string
  path?: string
  children?: MenuItem[]
}

export const mainMenuItems: MenuItem[] = [
  {
    id: 'products',
    label: 'Sản phẩm',
    children: [
      {
        id: 'aluminum-doors',
        label: 'Cửa nhôm',
        children: [
          {
            id: 'doors',
            label: 'Cửa đi',
            path: '/products/doors',
          },
          {
            id: 'windows',
            label: 'Cửa sổ',
            path: '/products/windows',
          },
        ],
      },
      {
        id: 'accessories',
        label: 'Phụ kiện',
        children: [
          {
            id: 'handles',
            label: 'Tay nắm cửa',
            path: '/accessories/handles',
          },
          {
            id: 'locks',
            label: 'Khóa cửa',
            path: '/accessories/locks',
          },
        ],
      },
    ],
  },
]

export const accountMenuItems: MenuItem[] = [
  {
    id: 'login',
    label: 'Đăng nhập',
    path: '/login',
  },
  {
    id: 'register',
    label: 'Đăng ký',
    path: '/register',
  },
]