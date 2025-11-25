import { Attribute } from '@/types/Product';
import React from 'react';

interface Props {
  attr: Attribute;
}

const FixTextAttribute: React.FC<Props> = ({ attr }) => {
  return (
    <div className="text-sm text-gray-700">
      {attr.valueText}
    </div>
  );
};

export default FixTextAttribute;