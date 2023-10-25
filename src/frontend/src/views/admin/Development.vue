<template>
  <select class="form-select" v-model="selectedRoom">
    <option value="all">All rooms</option>
    <option v-for="room in rooms" :value="room.id">{{ room.name }}</option>
  </select>
  <select class="form-select" v-model="selectedType">
    <option value="all">All device types</option>
    <option value="heating">Heating</option>
    <option value="plugs">Plugs</option>
    <option value="lights">Lights</option>
    <option value="sensors">Sensors</option>
    <option value="shutters">Shutters</option>
  </select>
  <div class="card-columns">
    <div class="card mb-1 p-2" v-for="device in devices.filter(device => (selectedRoom == 'all' || device.roomId == selectedRoom) && (selectedType == 'all' || (selectedType == 'heating' && (device.type == 'trv' || device.type == 'fancoil')) || (selectedType == 'plugs' && device.type == 'outlet') || (selectedType == 'lights' && device.type == 'light') || (selectedType == 'sensors' && (device.type == 'temperature' || device.type == 'leak')) || (selectedType == 'shutters' && device.type == 'shutter')))">
      <div class="card-body">
        <span :class="device.reachable ? 'online' : 'offline'" v-if="device.reachable != undefined" class="trafficlight">
          <svg class="bi me-2" width="16" height="16">
            <use xlink:href="#circle-fill"/>
          </svg>
        </span>
        <h5 class="card-title">{{ device.name }}</h5>
        <div v-if="device.type == 'temperature'">
          <h6 class="card-subtitle mb-2 text-muted">Temperature sensor</h6>
          <p class="fw-bold fs-4 mb-1">
              {{ device.temperature }} &deg;C <span class="explain">temperature</span>
          </p>
          <p class="fw-bold fs-4 mb-1">
              {{ device.pressure }} hPa <span class="explain">pressure</span>
          </p>
          <p class="fw-bold fs-4 mb-1">
              {{ device.humidity }}&#37; <span class="explain">humidity</span>
          </p>
        </div> 
        <div v-if="device.type == 'outlet'">
          <h6 class="card-subtitle mb-2 text-muted">Outlet</h6>
          <p class="fw-bold fs-4 mb-1" v-if="device.current != undefined">
              {{ device.current }} A <span class="explain">current</span>
          </p>
          <p class="fw-bold fs-4 mb-1" v-if="device.power != undefined">
              {{ device.power }} W <span class="explain">power</span>
          </p>
          <p class="fw-bold fs-4 mb-1" v-if="device.voltage != undefined">
              {{ device.voltage }} V <span class="explain">voltage</span>
          </p>
          <p class="fw-bold fs-4 mb-1" v-if="device.energy != undefined">
              {{ device.energy }} kWh <span class="explain">energy</span>
          </p>
          <div class="form-check form-switch">
            <input class="form-check-input" type="checkbox" :checked="device.on" @change="updateOnOff(device)" :disabled="!device.reachable || device.locked">
          </div>
          <div class="info" v-if="device.active != undefined">
            <span class="label">Active:</span> {{ device.active }}
          </div>
        </div> 
        <div v-if="device.type == 'leak'">
          <h6 class="card-subtitle mb-2 text-muted">Water leak sensor</h6>
          <p class="fw-bold fs-4 mb-1">
              <span v-if="device.leak == true">Leaking!</span>
              <span v-else>No leak</span>
          </p>
        </div>
        <div v-if="device.type == 'switch'">
          <h6 class="card-subtitle mb-2 text-muted">Switch</h6>
          <div class="info" v-if="device.action != undefined">
            <span class="label">Action:</span> {{ device.action }}
          </div>
          <div class="info" v-if="device.brightness != undefined">
            <span class="label">Brightness:</span> {{ device.brightness }}
          </div>
          <div class="info" v-if="device.buttonEvent != undefined">
            <span class="label">Button event:</span> {{ device.buttonEvent }}
          </div>
          <div class="info" v-if="device.sensorUpdate != undefined">
            <span class="label">Sensor update:</span> {{ device.sensorUpdate }}
          </div>
        </div>
        <div v-if="device.type == 'trv'">
          <h6 class="card-subtitle mb-2 text-muted">Heater</h6>
          <p class="fw-bold fs-4 mb-1">
              {{ device.temperature }} &deg;C <span class="explain">temperature</span>
          </p>
          <p class="fw-bold fs-4 mb-1">
              {{ device.requestedTemperature }} &deg;C <span class="explain">requested temperature</span>
          </p>
          <p class="fw-bold fs-4 mb-1">
              {{ device.valvePosition }}&#37; <span class="explain">valve position</span>
          </p>
          <p class="fw-bold fs-4 mb-1">
              <span v-if="device.isHeating == true">Heating</span>
              <span v-else>Not heating</span>
          </p>
          <div class="form-check form-switch">
            <input class="form-check-input" type="checkbox" :checked="device.on" @change="updateOnOff(device)" :disabled="!device.reachable || device.locked">
          </div>
          <input type="range" class="form-range" min="5" max="30" :value="device.requestedTemperature" @change="setRequestedTemperature($event, device)">
        </div>
        <div v-if="device.type == 'light'">
          <h6 class="card-subtitle mb-2 text-muted">Light</h6>
          <div class="form-check form-switch">
            <input class="form-check-input" type="checkbox" :checked="device.on" @change="updateOnOff(device)" :disabled="!device.reachable">
          </div>
          <input type="range" class="form-range" min="0" max="255" :value="device.brightness" v-if="device.brightness != undefined" @change="setBrightness($event, device)">
          <input type="range" class="form-range" min="153" max="500" :value="device.colorTemperature" v-if="device.colorTemperature != undefined" @change="setColorTemperature($event, device)">
          <input type="color" :value="getColorRgb(device.colorXy, device.brightness)" v-if="device.colorXy != undefined" @change="setColor(getColorXy($event.target.value), device)" />
        </div>
        <div v-if="device.type == 'hub'">
          <h6 class="card-subtitle mb-2 text-muted">Hub</h6>
        </div>
        <div v-if="device.type == 'solar'">
          <h6 class="card-subtitle mb-2 text-muted">Solar inverter</h6>
          <p class="fw-bold fs-4 mb-1" v-if="device.lifeTimeEnergy != undefined">
              {{ device.lifeTimeEnergy }} kWh <span class="explain">lifetime</span>
          </p>
          <p class="fw-bold fs-4 mb-1" v-if="device.lastYearEnergy != undefined">
              {{ device.lastYearEnergy }} kWh <span class="explain">last year</span>
          </p>
          <p class="fw-bold fs-4 mb-1" v-if="device.lastMonthEnergy != undefined">
              {{ device.lastMonthEnergy }} kWh <span class="explain">last month</span>
          </p>
          <p class="fw-bold fs-4 mb-1" v-if="device.lastDayEnergy != undefined">
              {{ device.lastDayEnergy }} kWh <span class="explain">last day</span>
          </p>
          <p class="fw-bold fs-4 mb-1" v-if="device.currentPower != undefined">
              {{ device.currentPower }} W <span class="explain">power</span>
          </p>
          <p class="fw-bold fs-4 mb-1" v-if="device.previousQuarterEnergy != undefined">
              {{ device.previousQuarterEnergy }} Wh <span class="explain">15 min energy</span>
          </p>
          <p class="fw-bold fs-4 mb-1" v-if="device.previousQuarterPower != undefined">
              {{ device.previousQuarterPower }} W <span class="explain">15 min power</span>
          </p>
        </div>
        <div v-if="device.type == 'fancoil'">
          <h6 class="card-subtitle mb-2 text-muted">Fancoil</h6>
          <p class="fw-bold fs-4 mb-1">
              {{ device.temperature }} &deg;C <span class="explain">temperature</span>
          </p>
          <p class="fw-bold fs-4 mb-1">
              {{ device.requestedTemperature }} &deg;C <span class="explain">requested temperature</span>
          </p>
          <div class="form-check form-switch">
            <input class="form-check-input" type="checkbox" :checked="device.on" @change="updateOnOff(device)" :disabled="!device.reachable || device.locked">
          </div>
          <input type="range" class="form-range" min="5" max="30" :value="device.requestedTemperature" @change="setRequestedTemperature($event, device)">
          <div class="btn-group" role="group">
            <input type="radio" class="btn-check" :name="device.deviceId + '-radio-speed'" :id="device.deviceId + '-radio-speed-1'" autocomplete="off" :checked="device.speed == 'Low'" @change="setSpeed(device, 'Low')">
            <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-speed-1'">Low</label>
            <input type="radio" class="btn-check" :name="device.deviceId + '-radio-speed'" :id="device.deviceId + '-radio-speed-2'" autocomplete="off" :checked="device.speed == 'Medium'" @change="setSpeed(device, 'Medium')">
            <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-speed-2'">Medium</label>
            <input type="radio" class="btn-check" :name="device.deviceId + '-radio-speed'" :id="device.deviceId + '-radio-speed-3'" autocomplete="off" :checked="device.speed == 'High'" @change="setSpeed(device, 'High')">
            <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-speed-3'">High</label>
            <input type="radio" class="btn-check" :name="device.deviceId + '-radio-speed'" :id="device.deviceId + '-radio-speed-4'" autocomplete="off" :checked="device.speed == 'Auto'" @change="setSpeed(device, 'Auto')">
            <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-speed-4'">Auto</label>
          </div>
          <div class="btn-group" role="group">
            <input type="radio" class="btn-check" :name="device.deviceId + '-radio-mode'" :id="device.deviceId + '-radio-mode-1'" autocomplete="off" :checked="device.mode == 'Heating'" @change="setMode(device, 'Heating')">
            <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-mode-1'">Heating</label>
            <input type="radio" class="btn-check" :name="device.deviceId + '-radio-mode'" :id="device.deviceId + '-radio-mode-2'" autocomplete="off" :checked="device.mode == 'Cooling'" @change="setMode(device, 'Cooling')">
            <label class="btn btn-outline-primary" :for="device.deviceId + '-radio-mode-2'">Cooling</label>
          </div>
        </div>
        <div v-if="device.type == 'shutter'">
          <h6 class="card-subtitle mb-2 text-muted">Shutter</h6>
          <input type="range" class="form-range" min="0" max="100" :value="device.position" v-if="device.position != undefined" @change="setPosition($event, device)">
          <div class="btn-group" role="group">
            <button type="button" class="btn btn-outline-primary" @click="up($event, device)">Up</button>
            <button type="button" class="btn btn-outline-danger" @click="stop($event, device)">Stop</button>
            <button type="button" class="btn btn-outline-primary" @click="down($event, device)">Down</button>
          </div>
        </div>
        <div class="info">
          <span class="label">Manufacturer:</span> {{ device.manufacturer }}
        </div>
        <div class="info">
          <span class="label">Model:</span> {{ device.model ?? "n/a" }}
        </div>
        <div class="info">
          <span class="label">Version:</span> {{ device.version ?? "n/a" }}
        </div>
        <div class="info" v-if="device.lastSeen != undefined">
          <span class="label">Last seen:</span> {{ getAgo(device.lastSeen) }}
        </div>
        <div class="info">
          <span class="label">Device ID:</span> {{ device.deviceId }}
        </div>
        <div class="info">
          <span class="label">Friendly ID:</span> {{ device.friendlyId }}
        </div>
        <div class="info" v-if="device.powerSource != undefined">
          <span class="label">Power source:</span> {{ device.powerSource }}
        </div>
        <div class="info" v-if="device.battery != undefined">
          <span class="label">Battery:</span> {{ device.battery }}&#37;
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState } from 'vuex'
import api from '../../api'
export default {
  data() {
    return {
      selectedRoom: "all",
      selectedType: "all",
    };
  },
  computed: mapState({
    rooms: state => state.rooms.all,
    devices: state => state.devices.all
  }),
  created () {
    this.$store.dispatch('getRooms')
    this.$store.dispatch('getDevices')
    var self = this
    setInterval(function() {
      self.$forceUpdate()
    }, 1000)
  },
  methods: {
    updateOnOff: function (device) {
      if (device.on) {
        api.sendDeviceCommand(device.deviceId, "turnOff")
      } else {
        api.sendDeviceCommand(device.deviceId, "turnOn")
      }
    },
    setRequestedTemperature: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "setRequestedTemperature", {
        t: event.target.value
      })
    },
    setMode: function (device, mode) {
      api.sendDeviceCommand(device.deviceId, "setMode", {
        m: mode
      })
    },
    setSpeed: function (device, speed) {
      api.sendDeviceCommand(device.deviceId, "setSpeed", {
        s: speed
      })
    },
    setBrightness: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "setBrightness", {
        bri: event.target.value
      })
    },
    setColorTemperature: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "setColorTemperature", {
        ct: event.target.value
      })
    },
    // TODO - setColor not working yet
    setColor: function (xy, device) {
      api.sendDeviceCommand(device.deviceId, "setColor", {
        X: xy.x,
        Y: xy.y
      })
    },
    setPosition: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "moveToTarget", {
        t: event.target.value
      })
    },
    up: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "moveUp")
    },
    stop: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "stop")
    },
    down: function (event, device) {
      api.sendDeviceCommand(device.deviceId, "moveDown")
    }
  }
}
</script>

<style scoped>
@import '../../assets/card.css';
.card {
  margin-top: 10px;
}
.online {
  color: green;
}
.offline {
  color: red;
}
.trafficlight {
  float: right;
}
.label {
  width: 100px;
  font-weight: bold;
  color: gray;
  display: inline-block;
}
.info {
  font-size: smaller;
}
.explain {
  color: gray;
  font-size: small;
}
select {
  width: calc(50% - 5px);
  display: inline-block;
}
select:first-of-type {
  margin-right: 10px;
}
.btn-group {
  margin-bottom: 10px;
}
input[type=range], input[type=color] {
  margin-top: 20px;
  margin-bottom: 20px;
  width: 100%
}
.card-columns {
  column-count: 2;
}
@media (min-width: 768px) {
  .card-columns {
    column-count: 3;
  }
}
@media (min-width: 992px) {
  .card-columns {
    column-count: 4;
  }
}
@media (min-width: 1200px) {
  .card-columns {
    column-count: 5;
  }
}
</style>