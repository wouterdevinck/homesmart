<template>
  <div>
    <div class="row">
      <div class="btn-group" role="group">
        <button type="button" class="btn btn-outline-primary" @click="preset($event, 'watermeter', 'totalliters', 'diff', '1d', '1w', 'bar')">Daily water use</button>
        <button type="button" class="btn btn-outline-primary" @click="preset($event, 'watermeter', 'totalliters', 'diff', '1w', '1mo', 'bar')">Weekly water use</button>
        <button type="button" class="btn btn-outline-primary" @click="preset($event, 'solar', 'lastmonthenergy', 'diff', '1d', '1w', 'bar')">Daily solar</button>
        <button type="button" class="btn btn-outline-primary" @click="preset($event, 'solar', 'lastyearenergy', 'diff', '1mo', '1y', 'bar')">Monthly solar</button>
        <button type="button" class="btn btn-outline-primary" @click="preset($event, 'plug-tv', 'energy', 'diff', '1d', '1w', 'bar')">Daily energy use of a plug</button>
        <button type="button" class="btn btn-outline-primary" @click="preset($event, 'sensor-living', 'temperature', 'mean', '1h', '12h', 'line')">Living room temperature</button>
      </div>
    </div>
    <div class="row mt-3">
      <div class="col">
        <p class="fw-bold mb-1">Device</p>
        <select class="form-select" v-model="selectedDevice">
          <option v-for="device in metadata" :value="device.deviceId">{{ device.deviceId }}</option>
        </select>
      </div>
      <div class="col">
        <p class="fw-bold mb-1">Point</p>
        <select class="form-select" v-model="selectedPoint">
          <option v-for="point in points" :value="point">{{ point }}</option>
        </select>
      </div>
      <div class="col">
        <p class="fw-bold mb-1">Windowing mode</p>
        <select class="form-select" v-model="selectedMode">
          <option value="none">None</option>
          <option value="diff">Difference</option>
          <option value="mean">Mean</option>
        </select>
      </div>
      <div class="col">
        <p class="fw-bold mb-1">Window</p>
        <select class="form-select" v-model="selectedWindow" :disabled="selectedMode == 'none'">
          <option value="1m">1 minute</option>
          <option value="5m">5 minutes</option>
          <option value="15m">15 minutes</option>
          <option value="30m">30 minutes</option>
          <option value="1h">1 hour</option>
          <option value="6h">6 hours</option>
          <option value="12h">12 hours</option>
          <option value="1d">1 day</option>
          <option value="1w">1 week</option>
          <option value="1mo">1 month</option>
          <option value="1y">1 year</option>
        </select>
      </div>
    </div>
    <div class="row mt-3">
      <div class="col">
        <p class="fw-bold mb-1">Range type</p>
        <select class="form-select" v-model="selectedRangeType">
          <option value="relative">Relative</option>
          <option value="absolute">Absolute</option>
        </select>
      </div>
      <div class="col">
        <div v-if="selectedRangeType == 'relative'">
          <p class="fw-bold mb-1">Relative since</p>
          <select class="form-select" v-model="selectedSince">
            <option value="12h">12 hours</option>
            <option value="1d">1 day</option>
            <option value="1w">1 week</option>
            <option value="1mo">1 month</option>
            <option value="1y">1 year</option>
          </select>          
        </div>
        <div v-else>
          <p class="fw-bold mb-1">Absolute from</p>
          TODO
        </div>
      </div>
      <div class="col">
        <div v-if="selectedRangeType == 'relative'">
          <p class="fw-bold mb-1">Relative to</p>
          TODO          
        </div>
        <div v-else>
          <p class="fw-bold mb-1">Absolute to</p>
          TODO
        </div>
      </div>
      <div class="col"></div>
    </div>
    <div class="row mt-3">
      <div class="col">
        <p class="fw-bold mb-1">Chart type</p>
        <select class="form-select" v-model="selectedChartType">
          <option value="bar">Bar</option>
          <option value="line">Line</option>
        </select>
      </div>
      <div class="col"></div>
      <div class="col"></div>
      <div class="col"></div>
    </div>
    <div class="spinner-border mt-5" role="status" v-if="fetching">
      <span class="visually-hidden">Loading...</span>
    </div>
    <div class="row mt-3" v-if="!fetching">
      <div class="col">
        <BarChart :data="data" class="mt-5" v-if="selectedChartType == 'bar'" />
        <LineChart :data="data" class="mt-5" v-if="selectedChartType == 'line'" />
      </div>
      <div class="col">
        <table class="table">
          <thead>
            <tr>
              <th scope="col">Timestamp</th>
              <th scope="col">Value</th>
            </tr>
          </thead>
          <tbody>
            <template v-for="item in data">
              <tr>
                <td>{{ item.time }}</td>
                <td>{{ item.value }}</td>
              </tr>
            </template>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState } from 'vuex'
import api from '../../api'
import BarChart from '../../components/admin/BarChart.vue'
import LineChart from '../../components/admin/LineChart.vue'
export default {
  components: { BarChart, LineChart },
  data() {
    return {
      selectedDevice: '',
      selectedPoint: '',
      nextSelectedPoint: null,
      selectedMode: 'none',
      selectedWindow: '1d',
      selectedSince: '1d',
      selectedRangeType: 'relative',
      selectedChartType: 'bar',
      data: [],
      fetching: false
    };
  },
  computed: {
    ...mapState({
      metadata: state => state.data.metadata
    }),
    points() {
      return this.metadata.filter(x => x.deviceId == this.selectedDevice)[0]?.points
    },
    dataId() {
      return `${this.selectedDevice}-${this.selectedPoint}-${this.selectedMode}-${this.selectedWindow}-${this.selectedSince}`
    }
  },
  created () {
    this.$store.dispatch('getMetadata')
  },
  watch: {
    metadata() {
      this.selectedDevice = this.metadata[0].deviceId
    },
    selectedDevice() {
      if(this.nextSelectedPoint) {
        this.selectedPoint = this.nextSelectedPoint
        this.nextSelectedPoint = null
      } else if (!this.selectedPoint) {
        this.selectedPoint = this.points[0]
      }
    },
    dataId() {
      this.getData()
    }
  },
  methods: {
    getData() {
      if(this.selectedDevice && this.selectedPoint) {
        this.fetching = true
        let diffWindow = null
        let meanWindow = null
        if(this.selectedMode == 'diff') {
          diffWindow = this.selectedWindow
        } else if (this.selectedMode == 'mean') {
          meanWindow = this.selectedWindow
        }
        api.getData(data => {
          this.data = data
          this.fetching = false
        }, this.selectedDevice, this.selectedPoint, 
        diffWindow, meanWindow, this.selectedSince)
      }
    },
    preset: function (event, device, point, mode, window, since, chartType) {
      this.selectedDevice = device
      this.selectedPoint = point
      this.nextSelectedPoint = this.selectedPoint
      this.selectedMode = mode
      this.selectedWindow = window
      this.selectedSince = since
      this.selectedChartType = chartType
    },
  }
}
</script>