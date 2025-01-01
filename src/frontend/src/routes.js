export const routes = [
  {
    path: '/',
    meta: { title: 'Homesmart', navigation: false },
    component: () => import('./views/app/Rooms.vue')
  },{
    path: '/rooms/:id',
    meta: { title: 'Homesmart', navigation: false/*, transition: 'fade' */},
    component: () => import('./views/app/Room.vue'),
    children: [
      {
        path: 'lights',
        meta: { subPage: true },
        component: () => import('./views/app/Lights.vue')
      },{
        path: 'climate',
        meta: { subPage: true },
        component: () => import('./views/app/Climate.vue')
      },{
        path: 'shutters',
        meta: { subPage: true },
        component: () => import('./views/app/Shutters.vue')
      },{
        path: 'energy',
        meta: { subPage: true },
        component: () => import('./views/app/Energy.vue')
      },{
        path: 'cameras',
        meta: { subPage: true },
        component: () => import('./views/app/Cameras.vue')
      }
    ]
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
  }, {
    path: '/data',
    meta: { title: 'Data', navigation: true },
    component: () => import('./views/admin/Data.vue')
  },
]