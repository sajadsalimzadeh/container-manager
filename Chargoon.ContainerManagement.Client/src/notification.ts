import { ReactNotificationOptions } from "react-notifications-component";

export const notificationOptions: ReactNotificationOptions = {
    insert: 'top',
    container: 'top-right',
    animationIn: ['animated', 'slideInDown'],
    animationOut: ['animated', 'fadeOut'],
    dismiss: {
        duration: 10000,
        onScreen: true
    }
}