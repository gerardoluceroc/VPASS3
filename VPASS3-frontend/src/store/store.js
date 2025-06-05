import { combineReducers, configureStore } from "@reduxjs/toolkit";
import { FLUSH, PAUSE, PERSIST, persistReducer, persistStore, PURGE, REGISTER, REHYDRATE } from 'redux-persist';
import storage from 'redux-persist/lib/storage';
import { authReducer } from "./misSlice";
import axios from "axios";



// Configuración de persistencia para redux-persist
const persistConfig = {
    key: 'root',        // Esta será la "clave raíz" usada en localStorage (por ejemplo: persist:root)
    storage: storage,   // Define el tipo de almacenamiento, en este caso localStorage (por defecto)
};

// Combina múltiples reducers en uno solo
const combinedReducer = combineReducers({
    user: authReducer, // 'user' será la clave del estado donde se guarda lo que maneja 'authReducer'
});

// Este es el rootReducer personalizado, que incluye una lógica para resetear el estado
const rootReducer = (state, action) => {
    // Si se dispara una acción con tipo 'RESET'
    if (action.type === 'RESET') {
        storage.removeItem('persist:root'); // Borra el estado persistido del localStorage
        state = undefined; // Reinicia el estado completo (vuelve a los valores iniciales de los reducers)
    }
    // Devuelve el estado usando el reducer combinado
    return combinedReducer(state, action);
};

// Este reducer incluye la lógica de persistencia usando redux-persist
// Va a guardar (y restaurar) el estado del rootReducer usando la configuración definida
const persistedReducer = persistReducer(persistConfig, rootReducer);

// Crea el store de Redux, que contiene todo el estado global de la aplicación
// Usamos el reducer con persistencia y le agregamos el middleware thunk para manejar acciones asincrónicas
// export const store = configureStore({
//     reducer: persistedReducer, // El reducer principal del store (con persistencia incluida)
//     middleware: [thunk],       // Agrega soporte para acciones asíncronas (por ejemplo, llamadas a APIs)
// });

// Crea el store de Redux, que contiene todo el estado global de la aplicación
// Usamos el reducer con persistencia y le agregamos el midd230pleware thunk para manejar acciones asincrónicas
// Aquí está la solución al warning4r
export const store = configureStore({
    reducer: persistedReducer,
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware({
        thunk: true,
        serializableCheck: {
          // Ignora estas acciones internas de redux-persist
          ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
        },
      }),
  });

// Crea el persistor, que se encarga de controlar el proceso de guardar y restaurar el estado persistido
// export const persistor = persistStore(store);





// Crea el persistor
export const persistor = persistStore(store, null, () => {
  // Este callback se ejecuta una vez que el store está rehidratado.
  // Aquí podemos configurar el interceptor o asegurarnos de que el token esté disponible.
});

// --- ¡La clave para la solución está aquí! ---
// Configura un único interceptor de Axios y haz que reaccione a los cambios del token
let axiosInterceptorId = null; // Para almacenar el ID del interceptor y poder ejectarlo

// Inicializa el interceptor o actualiza el token cuando el store cambia
const configureAxiosInterceptor = () => {
    const state = store.getState();
    const token = state.user.token;

    // Si ya hay un interceptor, lo removemos para crear uno nuevo con el token actualizado
    if (axiosInterceptorId !== null) {
        axios.interceptors.request.eject(axiosInterceptorId);
    }

    axiosInterceptorId = axios.interceptors.request.use(
        (config) => {
            // Accede al token DIRECTAMENTE del estado actual del store
            const currentToken = store.getState().user.token;
            if (currentToken) {
                config.headers.Authorization = `Bearer ${currentToken}`;
            } else {
                delete config.headers.Authorization;
            }
            return config;
        },
        (error) => {
            return Promise.reject(error);
        }
    );
};

// Suscríbete a los cambios del store de Redux
let currentToken = null;
store.subscribe(() => {
    const previousToken = currentToken;
    currentToken = store.getState().user.token;

    // Solo re-configura el interceptor si el token realmente ha cambiado
    if (currentToken !== previousToken) {
        configureAxiosInterceptor();
    }
});

// Asegúrate de configurar el interceptor al inicio con el token inicial (o null si no hay)
// Esto se ejecutará una vez que el store se inicialice/rehidrate.
configureAxiosInterceptor();
