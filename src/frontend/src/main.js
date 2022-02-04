import { createApp } from 'vue'
import { routes } from './routes.js'
import store from './store'
import icons from './mixins/icons.js'
import ago from './mixins/ago.js'
import { createRouter, createWebHistory } from 'vue-router'
import App from './App.vue'
import "bootstrap/dist/css/bootstrap.min.css"

const app = createApp(App)

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, _, next) => {
  document.title = to.meta.title || 'Home'
  next()
})

app.use(router)
app.use(store)

app.mixin(icons)
app.mixin(ago)

app.mount('#app')