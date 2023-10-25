import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { VitePWA } from 'vite-plugin-pwa'

// https://vitejs.dev/config/
export default defineConfig({

  plugins: [
  
    vue(),
  
    VitePWA({ 
      registerType: 'autoUpdate',
      injectRegister: 'auto',
      devOptions: {
        enabled: true
      },
      manifest: {
        name: "Homesmart"
      }
    })
  
  ],

  server: {
    port: 3000,
    host: '0.0.0.0'
  }

})
