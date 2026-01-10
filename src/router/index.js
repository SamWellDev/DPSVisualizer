import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import DashboardView from '../views/DashboardView.vue'
import OverlayView from '../views/OverlayView.vue'
import ShopView from '../views/ShopView.vue'

const routes = [
    {
        path: '/',
        name: 'login',
        component: LoginView
    },
    {
        path: '/dashboard',
        name: 'dashboard',
        component: DashboardView
    },
    {
        path: '/overlay',
        name: 'overlay',
        component: OverlayView
    },
    {
        path: '/shop',
        name: 'shop',
        component: ShopView
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router
