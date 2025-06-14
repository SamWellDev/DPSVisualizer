# Visualizador de DPS

Uma landing page interativa para visualizar e calcular DPS (Dano Por Segundo) em tempo real, desenvolvida com Vue 3 e Tailwind CSS.

## 🚀 Características

- **Interface Responsiva**: Layout adaptável para desktop e mobile
- **Visualização Interativa**: Área de visualização com animações de nave espacial e meteoro
- **Controles Dinâmicos**: Três sliders para ajustar parâmetros:
  - Number of Shots (1-10)
  - Rate of Fire (1-10) 
  - Power (1-10)
- **Cálculo em Tempo Real**: DPS calculado automaticamente baseado nos parâmetros
- **Tema Moderno**: Design limpo com cores branco e azul claro

## 🛠️ Tecnologias Utilizadas

- **Vue 3** com Composition API (`<script setup>`)
- **Vite** para build e desenvolvimento
- **Tailwind CSS** para estilização
- **JavaScript ES6+**

## 📦 Estrutura do Projeto

```
dps-visualizer/
├── src/
│   ├── components/
│   │   └── VisualizationArea.vue
│   ├── App.vue
│   ├── main.js
│   └── style.css
├── index.html
├── package.json
├── vite.config.js
├── tailwind.config.js
└── postcss.config.js
```

## 🎮 Como Usar

1. **Instalar dependências**:
   ```bash
   npm install
   ```

2. **Executar em modo desenvolvimento**:
   ```bash
   npm run dev
   ```

3. **Build para produção**:
   ```bash
   npm run build
   ```

## 🎯 Funcionalidades

### Área de Visualização
- Nave espacial animada com efeitos visuais
- Meteoro como alvo
- Disparos laser animados
- Efeitos de impacto
- Estrelas de fundo com animação
- Status da batalha em tempo real

### Controles
- Sliders customizados com design moderno
- Valores exibidos em tempo real
- Cálculo automático de DPS
- Interface intuitiva e responsiva

## 🎨 Design

O projeto utiliza um tema minimalista com:
- **Cor primária**: Azul (#3b82f6)
- **Background**: Branco e tons de azul claro
- **Tipografia**: Sans-serif moderna
- **Animações**: Transições suaves e micro-interações

## 📱 Responsividade

A aplicação é totalmente responsiva e se adapta a diferentes tamanhos de tela:
- **Desktop**: Layout de duas colunas lado a lado
- **Mobile**: Layout empilhado verticalmente
- **Tablet**: Adaptação automática baseada no tamanho da tela

