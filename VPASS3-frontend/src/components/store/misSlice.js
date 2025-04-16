import { createSlice } from "@reduxjs/toolkit";

const authSlice = createSlice(
    {
        name: "authentication",
        initialState:
        {
            authenticated: false,
            token: null,
            rememberMe: false,
        },
        reducers:
        {
            setUser: (state, action) => {
                state.authenticated = action.payload.authenticated;
                state.token = action.payload.token;
                state.rememberMe = action.payload.rememberMe;
            },
            disconnect: (state) => {
                state.authenticated = false;
                state.token = null;
                state.rememberMe = false;
            },            
        }
    }
)
export const { setUser, disconnect } = authSlice.actions;
export const authReducer = authSlice.reducer;





// import { createSlice } from "@reduxjs/toolkit";

// const authSlice = createSlice({
//     name: "authentication", // Nombre del slice
//     initialState: {
//         authenticated: false,
//         token: null,
//         expiracion: null,
//         idEmpresa: null,
//         usuario: null,
//         correo: null,
//         idPersona: null,
//         rol: null,
//         rememberMe: false,
//     },
//     reducers: {
//         setUser: (state, action) => {
//             // Acá estás reemplazando todo con el mismo objeto payload
//             Object.assign(state, action.payload); // Mejor alternativa
//         },
//         disconnect: (state, action) => {
//             // Lo mismo aquí, deberías dejar el estado limpio
//             return {
//                 authenticated: false,
//                 token: null,
//                 expiracion: null,
//                 idEmpresa: null,
//                 usuario: null,
//                 correo: null,
//                 idPersona: null,
//                 rol: null,
//                 rememberMe: false,
//             };
//         }
//     }
// });

// export const { setUser, disconnect } = authSlice.actions;
// export const authReducer = authSlice.reducer;
