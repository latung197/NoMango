# Nitto1 Task Flow

## 1. Move Robot

Webhook Complete Data: `1_Complete`

## 2. Move Robot

Webhook Complete Data: `3_Complete`

## 3. Robot Lift

Webhook Complete Data: `4_Complete`

## 4. Move Robot (Lift Up)

Webhook Complete Data: `5_Complete`

## 5. Move Robot (Lift Up)

Webhook Complete Data: `7_Complete`

## 5.5. Move Robot (Lift Up) — Drop waiting buffer

Webhook Complete Data: `71_Complete`

- Case thường: nextPositionCode = ToStation.WaitingPoint (giữ nguyên vị trí)
- Case redirect: nextPositionCode = fallbackStation.WaitingPoint (đổi trạm hạ hàng)

## 6. Move Robot (Lift Up)

Webhook Complete Data: `9_Complete`

## 7. Robot Down

Webhook Complete Data: `10_Complete`

## 8. Move Robot

Webhook Complete Data: `11_Complete`
