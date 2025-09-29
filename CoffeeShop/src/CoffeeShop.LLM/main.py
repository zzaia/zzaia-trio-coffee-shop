"""
CoffeeShop LLM Service - FastAPI backend for AI-powered features
"""
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI(
    title="CoffeeShop LLM Service",
    description="AI-powered backend service for coffee shop recommendations and customer service",
    version="1.0.0"
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.get("/")
async def health_check():
    """Health check endpoint"""
    return {"status": "healthy", "service": "CoffeeShop LLM"}

@app.get("/health")
async def health():
    """Health endpoint for monitoring"""
    return {"status": "ok"}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)