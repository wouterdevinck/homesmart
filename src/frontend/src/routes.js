export const routes = [
  { 
    path: '/', 
    meta: { title: 'Dashboard' }, 
    component: () => import('./views/Dashboard.vue')
  }, {
    path: '/devices',
    meta: { title: 'Devices' },
    component: () => import('./views/DeviceList.vue')
  }, {
    path: '/automations',
    meta: { title: 'Automations' },
    component: () => import('./views/AutomationList.vue')
  }, {
    path: '/battery',
    meta: { title: 'Battery' },
    component: () => import('./views/Battery.vue')
  }, {
    path: '/development',
    meta: { title: 'Development' },
    component: () => import('./views/Development.vue')
  }
]