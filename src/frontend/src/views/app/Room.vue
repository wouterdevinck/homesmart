<template>
  <div v-if="room">
    <router-link to="/">
      <div class="roomNavigation mb-3">
        <svg class="bi me-3" width="20" height="20"><use xlink:href="#arrow-left-circle-fill"/></svg>
        <b>{{ room.name }}</b>
      </div>
    </router-link>
    <div class="btn-group d-flex mb-3" role="group">   
      <router-link to="lights" class="btn btn-light w-100" active-class="active" v-if="hasLights">
        <svg class="bi m-1" width="24" height="24"><use xlink:href="#lightbulb"/></svg>
        <div class="label">Lichten</div>
      </router-link>
      <router-link to="climate" class="btn btn-light w-100" active-class="active" v-if="hasClimate">
        <svg class="bi m-1" width="24" height="24"><use xlink:href="#thermometer-half"/></svg>
        <div class="label">Klimaat</div>
      </router-link>
      <router-link to="shutters" class="btn btn-light w-100" active-class="active" v-if="hasShutters">
        <svg class="bi m-1" width="24" height="24"><use xlink:href="#archive"/></svg>
        <div class="label">Rolluiken</div>
      </router-link>
      <router-link to="energy" class="btn btn-light w-100" active-class="active" v-if="hasEnergy">
        <svg class="bi m-1" width="24" height="24"><use xlink:href="#lightning-charge"/></svg>
        <div class="label">Energie</div>
      </router-link>
    </div>
    <router-view></router-view>
  </div>
</template>
  
<script>
import { mapState } from 'vuex'
export default {
  computed: {
    ...mapState({
      allRooms: state => state.rooms.all,
      allDevices: state => state.devices.all
    }),
    room() {
      var roomId = this.$route.params.id
      return this.allRooms.find(room => room.id == roomId)
    },
    devices() {
      var roomId = this.$route.params.id
      return this.allDevices.filter(device => device.roomId == roomId)
    },
    hasLights() {
      return this.devices.some(device => device.type == 'light' || (device.type == 'outlet' && !device.energy))
    },
    hasClimate() {
      return this.devices.some(device => device.type == 'fancoil' || device.type == 'trv' || device.type == 'temperature')
    },
    hasShutters() {
      return this.devices.some(device => device.type == 'shutter')
    },
    hasEnergy() {
      return this.devices.some(device => (device.type == 'outlet' && device.energy) || device.type == 'solar')
    }
  },
  created () {
    this.$store.dispatch('getRooms')
    this.$store.dispatch('getDevices')
    this.$watch(
      () => this.devices,
      (_, __) => {
        this.tryGoToFirstPageIfNeeded()
      }
    )
    this.tryGoToFirstPageIfNeeded()
  },
  methods: {
    tryGoToFirstPageIfNeeded: function() {
      if (!this.$route.meta.subPage) {
        let page = ''
        if (this.hasLights) {
          page = 'lights'
        } else if (this.hasClimate) {
          page = 'climate'
        } else if (this.hasShutters) {
          page = 'shutters'
        } else if (this.hasEnergy) {
          page = 'energy'
        }
        if (page) {
          let path = `/rooms/${this.$route.params.id}/${page}`
          this.$router.replace(path)
        }
      }
    }
  }
}
</script>
  
<style scoped>
a {
  text-decoration: none;
}
.roomNavigation {
  margin-bottom: 15px;
}
.roomNavigation > b {
  color: black;
  font-size: x-large;
}
.roomNavigation > * {
  vertical-align: middle;
}
.label {
  font-size: smaller;
  text-align: center;
}
@media not (hover: hover) {
    .btn:hover {
        background-color: var(--bs-btn-bg);
        border-color: var(--bs-btn-border-color);
    }
    .active:hover {
        background-color: var(--bs-btn-active-bg);
        border-color: var(--bs-btn-active-border-color);
    }
}
</style>