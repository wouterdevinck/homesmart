<template>
  <div id="wrapper">
    <Icons />
    <Navigation v-if="$route.meta.navigation" />
    <main class="p-3">
      <router-view></router-view>
      <!--<router-view v-slot="{ Component, route }">        
        <transition :name="route.meta.transition">
          <component :is="Component" :key="route.path" />
        </transition>
      </router-view>-->
    </main>
  </div>
</template>

<script>
import Icons from './components/common/Icons.vue'
import Navigation from './components/common/Navigation.vue'
export default {
  components: { Icons, Navigation }
}
</script>

<style>
body {
  min-height: 100vh;
  min-height: -webkit-fill-available;
  min-height: -moz-available;
  min-height: fill-available;
}
html {
  height: -webkit-fill-available;
  height: -moz-available;
  height: fill-available;
}
#wrapper {
  display: flex;
  flex-wrap: nowrap;
  height: 100vh;
  height: -webkit-fill-available;
  height: -moz-available;
  height: fill-available;
  max-height: 100vh;
  min-height: 100vh;
  overflow-x: auto;
  overflow-y: hidden;
}
main {
  overflow-y: auto;
  width: 100%;
  width: -webkit-fill-available;
  width: -moz-available;
  width: fill-available;
}
/*.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}*/
</style>