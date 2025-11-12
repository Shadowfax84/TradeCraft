import { defineConfig } from 'vite';
import { readFileSync } from 'fs';
import { resolve } from 'path';

export default defineConfig({
  server: {
    port: 5173,
    open: true,
  },
  build: {
    outDir: 'dist',
    target: 'ES2022',
    sourcemap: false,
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['@angular/core', '@angular/common', '@angular/platform-browser'],
          material: ['@angular/material'],
        },
      },
    },
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
    },
  },
});
