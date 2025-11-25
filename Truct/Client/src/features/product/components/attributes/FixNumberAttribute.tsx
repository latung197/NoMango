import { Attribute } from '@/types/Product';
import React from 'react';

interface Props {
  attr: Attribute;
}

const FixNumberAttribute: React.FC<Props> = ({ attr }) => {
  return (
    <div className="text-sm text-gray-700">
      {attr.valueNumber} {attr.unit}
    </div>
  );
};

export default FixNumberAttribute;