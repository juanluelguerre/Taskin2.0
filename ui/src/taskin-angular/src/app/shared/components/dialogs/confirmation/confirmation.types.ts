export type UiPaletteColor =
    | 'primary'
    | 'accent'
    | 'warn'
    | 'basic'
    | 'info'
    | 'success'
    | 'warning'
    | 'error'

export interface UiConfirmationConfig {
    title?: string
    message?: string
    icon?: {
        show?: boolean
        name?: string
        color?: UiPaletteColor
    }
    actions?: {
        confirm?: {
            show?: boolean
            label?: string
            color?: 'primary' | 'accent' | 'warn'
        }
        cancel?: {
            show?: boolean
            label?: string
        }
    }
    dismissible?: boolean
}

export type UiConfirmationResult = 'confirmed' | 'cancelled' | undefined

