import { createSlice } from "@reduxjs/toolkit";

const authSlice = createSlice(
    {
        name: "authentication",
        initialState:
        {
            authenticated: false,
            token: null,
            rememberMe: false,
            expirationTokenTimestamp: null,
        },
        reducers:
        {
            setUser: (state, action) => {
                state.authenticated = action.payload.authenticated;
                state.token = action.payload.token;
                state.rememberMe = action.payload.rememberMe;
                state.expirationTokenTimestamp = action.payload.expirationTokenTimestamp;
            },
            disconnect: (state) => {
                state.authenticated = false;
                state.token = null;
                state.rememberMe = false;
                state.expirationTokenTimestamp = null;
            },            
        }
    }
)
export const { setUser, disconnect } = authSlice.actions;
export const authReducer = authSlice.reducer;