export const genderOptions = [
  { label: '男性', value: 'MALE' },   // Nam
  { label: '女性', value: 'FEMALE' }, // Nữ
  { label: 'その他', value: 'OTHER' } // Khác
];

export const getDefaultGender = () => genderOptions[0].value;
