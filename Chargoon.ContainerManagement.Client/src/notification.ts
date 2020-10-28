import { ReactNotificationOptions } from "react-notifications-component";

export const notificationOptions: ReactNotificationOptions = {
    insert: 'bottom',
    container: 'bottom-right',
    animationIn: ['animated', 'slideInUp'],
    animationOut: ['animated', 'fadeOut'],
    dismiss: {
        duration: 10000,
        onScreen: true
    }
}