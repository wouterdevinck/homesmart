export const routes = [
  {
    path: '/',
    meta: { title: 'Homesmart', navigation: false },
    component: () => import('./views/app/Rooms.vue')
  }, { 
    path: '/dashboard', 
    meta: { title: 'Dashboard', navigation: true }, 
    component: () => import('./views/admin/Dashboard.vue')
  }, {
    path: '/devices',
    meta: { title: 'Devices', navigation: true },
    component: () => import('./views/admin/DeviceList.vue')
  }, {
    path: '/automations',
    meta: { title: 'Automations', navigation: true },
    component: () => import('./views/admin/AutomationList.vue')
  }, {
    path: '/battery',
    meta: { title: 'Battery', navigation: true },
    component: () => import('./views/admin/Battery.vue')
  }, {
    path: '/development',
    meta: { title: 'Development', navigation: true },
    component: () => import('./views/admin/Development.vue')
  },
]