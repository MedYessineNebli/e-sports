import { useCallback, useEffect, useMemo, useState } from 'react'
import type { FormEvent, ReactNode } from 'react'
import axios from 'axios'
import {
  BarChart3,
  Bell,
  CalendarDays,
  ChevronRight,
  Gamepad2,
  LayoutDashboard,
  LogOut,
  Menu,
  Moon,
  Search,
  Settings,
  Shield,
  Swords,
  Sun,
  Trophy,
  Users,
  X,
} from 'lucide-react'
import './App.css'

type AuthResponse = {
  token: string
  expiration: string
  email: string
  roles: string[]
}

type Paged<T> = { items: T[]; totalCount: number; totalPages: number }
type Game = { id: number; name: string; description?: string | null }
type Team = { id: number; name: string }
type Tournament = { id: number; name: string; gameId: number; startDate: string; endDate: string }
type Match = {
  id: number
  teamAName: string
  teamBName: string
  scoreTeamA: number
  scoreTeamB: number
  scheduledAt: string
  playedAt?: string | null
}
type User = { id: string; userName: string; email: string; roles: string[] }

const api = axios.create({ baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5192/api' })
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('tournois.token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

const formatDate = (value: string) =>
  new Intl.DateTimeFormat('en', { month: 'short', day: 'numeric', year: 'numeric' }).format(new Date(value))

function App() {
  const [auth, setAuth] = useState<AuthResponse | null>(() => {
    const saved = localStorage.getItem('tournois.auth')
    return saved ? JSON.parse(saved) as AuthResponse : null
  })
  const [darkMode, setDarkMode] = useState(() => localStorage.getItem('tournois.theme') !== 'light')
  const [mobileMenu, setMobileMenu] = useState(false)
  const [active, setActive] = useState('Dashboard')
  const [toast, setToast] = useState<{ type: 'success' | 'error'; message: string } | null>(null)
  const [showTournamentForm, setShowTournamentForm] = useState(false)

  useEffect(() => {
    document.documentElement.dataset.theme = darkMode ? 'dark' : 'light'
    localStorage.setItem('tournois.theme', darkMode ? 'dark' : 'light')
  }, [darkMode])

  useEffect(() => {
    if (!toast) return
    const timer = window.setTimeout(() => setToast(null), 4200)
    return () => window.clearTimeout(timer)
  }, [toast])

  const logout = () => {
    localStorage.removeItem('tournois.token')
    localStorage.removeItem('tournois.auth')
    setAuth(null)
    setToast({ type: 'success', message: 'You have been signed out.' })
  }

  if (!auth) return <Login onAuthenticated={setAuth} onToast={setToast} />

  return (
    <div className="app-shell">
      <aside className={`sidebar ${mobileMenu ? 'sidebar-open' : ''}`}>
        <div className="brand">
          <div className="brand-mark"><Swords size={21} /></div>
          <div><strong>ARENA<span>HQ</span></strong><small>TOURNAMENT OS</small></div>
          <button className="icon-button mobile-close" onClick={() => setMobileMenu(false)} aria-label="Close menu"><X size={20} /></button>
        </div>
        <nav>
          <p className="nav-label">Workspace</p>
          <NavItem icon={<LayoutDashboard size={18} />} label="Dashboard" active={active} onClick={setActive} />
          <NavItem icon={<Trophy size={18} />} label="Tournaments" active={active} onClick={setActive} />
          <NavItem icon={<CalendarDays size={18} />} label="Matches" active={active} onClick={setActive} />
          <NavItem icon={<Users size={18} />} label="Teams & Players" active={active} onClick={setActive} />
          <NavItem icon={<Gamepad2 size={18} />} label="Games" active={active} onClick={setActive} />
          <p className="nav-label nav-label-spaced">Manage</p>
          <NavItem icon={<BarChart3 size={18} />} label="Analytics" active={active} onClick={setActive} />
          <NavItem icon={<Settings size={18} />} label="Settings" active={active} onClick={setActive} />
          {auth.roles.includes('Admin') && <NavItem icon={<Shield size={18} />} label="Users" active={active} onClick={setActive} />}
        </nav>
        <div className="sidebar-bottom">
          <div className="pro-card"><Shield size={17} /><div><strong>Organizer workspace</strong><small>All systems operational</small></div></div>
          <button className="profile" onClick={logout}><div className="avatar">{(auth.email[0] ?? 'A').toUpperCase()}</div><div className="profile-copy"><strong>{auth.email.split('@')[0]}</strong><small>{auth.roles[0] ?? 'Player'}</small></div><LogOut size={16} /></button>
        </div>
      </aside>
      {mobileMenu && <button className="sidebar-overlay" onClick={() => setMobileMenu(false)} aria-label="Close menu" />}
      <main className="main-content">
        <header className="topbar">
          <button className="icon-button mobile-menu" onClick={() => setMobileMenu(true)} aria-label="Open menu"><Menu size={21} /></button>
          <div className="breadcrumbs"><span>Workspace</span><ChevronRight size={14} /><strong>{active}</strong></div>
          <div className="topbar-actions"><div className="search"><Search size={17} /><input placeholder="Search anything..." aria-label="Search" /></div><button className="icon-button theme-toggle" onClick={() => setDarkMode((value) => !value)} aria-label="Toggle theme">{darkMode ? <Sun size={18} /> : <Moon size={18} />}</button><button className="icon-button notification" aria-label="Notifications"><Bell size={18} /><i /></button><button className="logout-button" onClick={logout}><LogOut size={16} /> Logout</button></div>
        </header>
        {active === 'Dashboard' ? <Dashboard onToast={setToast} onNavigate={setActive} onCreateTournament={() => setShowTournamentForm(true)} /> : active === 'Users' ? <UserManagementPage onToast={setToast} /> : <ResourcePage resource={active} onToast={setToast} onCreateTournament={() => setShowTournamentForm(true)} />}
      </main>
      {toast && <div className={`toast ${toast.type}`} role="alert"><span>{toast.type === 'success' ? '✓' : '!'}</span>{toast.message}<button onClick={() => setToast(null)} aria-label="Dismiss"><X size={15} /></button></div>}
      {showTournamentForm && <TournamentForm onClose={() => setShowTournamentForm(false)} onToast={setToast} />}
    </div>
  )
}

function NavItem({ icon, label, active, onClick }: { icon: ReactNode; label: string; active: string; onClick: (label: string) => void }) {
  return <button className={`nav-item ${active === label ? 'active' : ''}`} onClick={() => onClick(label)}>{icon}<span>{label}</span>{active === label && <i />}</button>
}

function Login({ onAuthenticated, onToast }: { onAuthenticated: (auth: AuthResponse) => void; onToast: (toast: { type: 'success' | 'error'; message: string }) => void }) {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const submit = async (event: FormEvent) => {
    event.preventDefault()
    if (!username.trim() || !password) return onToast({ type: 'error', message: 'Enter your username and password.' })
    setLoading(true)
    try {
      const { data } = await api.post<AuthResponse>('/auth/login', { userName: username.trim(), password })
      localStorage.setItem('tournois.token', data.token)
      localStorage.setItem('tournois.auth', JSON.stringify(data))
      onAuthenticated(data)
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 401) onToast({ type: 'error', message: 'Invalid credentials. Please try again.' })
      else onToast({ type: 'error', message: 'The API is unavailable. Check that the backend is running.' })
    } finally { setLoading(false) }
  }
  return <div className="login-page"><div className="login-art"><div className="art-glow" /><div className="login-brand"><div className="brand-mark"><Swords size={21} /></div><strong>ARENA<span>HQ</span></strong></div><div className="art-copy"><p className="eyebrow">COMPETE. CONNECT. CONQUER.</p><h1>Your arena.<br /><em>Your legacy.</em></h1><p>One command center for every tournament, team, and match.</p></div><div className="art-stat"><Trophy size={18} /><span><strong>1,248</strong> matches managed this season</span></div></div><div className="login-panel"><div className="login-form"><div className="mobile-login-logo"><div className="brand-mark"><Swords size={21} /></div><strong>ARENA<span>HQ</span></strong></div><p className="eyebrow">WELCOME BACK</p><h2>Sign in to your workspace</h2><p className="muted">Manage your tournaments with confidence.</p><form onSubmit={submit}><label>Username<input value={username} onChange={(event) => setUsername(event.target.value)} placeholder="Enter your username" autoComplete="username" /></label><label>Password<input type="password" value={password} onChange={(event) => setPassword(event.target.value)} placeholder="Enter your password" autoComplete="current-password" /></label><button className="primary-button full-width" disabled={loading}>{loading ? 'Signing in…' : 'Sign in'}<ChevronRight size={17} /></button></form><p className="login-hint">Need an account? <span>Ask your administrator</span></p></div></div></div>
}

function Dashboard({ onToast, onNavigate, onCreateTournament }: { onToast: (toast: { type: 'success' | 'error'; message: string }) => void; onNavigate: (label: string) => void; onCreateTournament: () => void }) {
  const [data, setData] = useState<{ games: Game[]; teams: Team[]; tournaments: Tournament[]; matches: Match[] } | null>(null)
  const [loading, setLoading] = useState(true)
  useEffect(() => {
    const load = async () => {
      try {
        const [games, teams, tournaments, matches] = await Promise.all([
          api.get<Paged<Game>>('/games?page=1&pageSize=100'),
          api.get<Paged<Team>>('/teams?page=1&pageSize=100'),
          api.get<Paged<Tournament>>('/tournaments?page=1&pageSize=100'),
          api.get<Paged<Match>>('/matches?page=1&pageSize=5'),
        ])
        setData({ games: games.data.items, teams: teams.data.items, tournaments: tournaments.data.items, matches: matches.data.items })
      } catch {
        onToast({ type: 'error', message: 'Could not load dashboard data. Showing an empty workspace.' })
        setData({ games: [], teams: [], tournaments: [], matches: [] })
      } finally { setLoading(false) }
    }
    void load()
  }, [onToast])
  const upcoming = useMemo(() => data?.matches.filter((match) => !match.playedAt).slice(0, 4) ?? [], [data])
  const activeTournaments = data?.tournaments.filter((item) => new Date(item.endDate) >= new Date()).slice(0, 3) ?? []
  return <div className="page"><div className="page-heading"><div><p className="eyebrow">MONDAY, NOVEMBER 16, 2026</p><h1>Good evening, organizer <span>✦</span></h1><p className="muted">Here’s what’s happening across your arena today.</p></div><button className="primary-button" onClick={onCreateTournament}><span>+</span> Create tournament</button></div><section className="stats-grid"><Stat icon={<Trophy />} label="Active tournaments" value={loading ? '—' : String(activeTournaments.length)} trend="+12.5%" color="purple" /><Stat icon={<Swords />} label="Upcoming matches" value={loading ? '—' : String(upcoming.length)} trend="+8.2%" color="blue" /><Stat icon={<Users />} label="Registered teams" value={loading ? '—' : String(data?.teams.length ?? 0)} trend="+18.4%" color="orange" /><Stat icon={<Gamepad2 />} label="Games supported" value={loading ? '—' : String(data?.games.length ?? 0)} trend="+4.1%" color="green" /></section><div className="dashboard-grid"><section className="panel matches-panel"><div className="panel-heading"><div><h3>Upcoming matches</h3><p className="muted">Your next scheduled battles</p></div><button className="text-button" onClick={() => onNavigate('Matches')}>View all <ChevronRight size={15} /></button></div>{upcoming.length ? <div className="match-list">{upcoming.map((match) => <div className="match-row" key={match.id}><div className="match-date"><strong>{new Date(match.scheduledAt).getDate()}</strong><span>{new Date(match.scheduledAt).toLocaleString('en', { month: 'short' })}</span></div><div className="match-teams"><strong>{match.teamAName}</strong><span>vs</span><strong>{match.teamBName}</strong></div><div className="match-time">{new Date(match.scheduledAt).toLocaleTimeString('en', { hour: 'numeric', minute: '2-digit' })}<small>Scheduled</small></div><button className="more-button" aria-label="Match options">•••</button></div>)}</div> : <EmptyState label="No upcoming matches yet." />}</section><section className="panel activity-panel"><div className="panel-heading"><div><h3>Activity overview</h3><p className="muted">Tournament performance</p></div><select aria-label="Activity period"><option>Last 30 days</option><option>Last 7 days</option></select></div><div className="chart"><div className="chart-y"><span>40</span><span>30</span><span>20</span><span>10</span><span>0</span></div><div className="chart-area"><div className="grid-lines"><i /><i /><i /><i /><i /></div><div className="bars">{[38, 52, 44, 66, 58, 76, 63, 86, 70, 91, 80, 96].map((height, index) => <i key={index} style={{ height: `${height}%` }} />)}</div><div className="chart-labels">{['May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'].map((month) => <span key={month}>{month}</span>)}</div></div></div></section></div><section className="panel tournaments-panel"><div className="panel-heading"><div><h3>Active tournaments</h3><p className="muted">Keep an eye on your live competitions</p></div><button className="text-button" onClick={() => onNavigate('Tournaments')}>Manage tournaments <ChevronRight size={15} /></button></div><div className="tournament-table"><div className="table-head"><span>Tournament</span><span>Dates</span><span>Status</span><span>Progress</span></div>{activeTournaments.length ? activeTournaments.map((tournament, index) => <div className="table-row" key={tournament.id}><div className="tournament-name"><div className={`game-icon game-${index % 3}`}><Gamepad2 size={16} /></div><strong>{tournament.name}</strong></div><span className="table-date">{formatDate(tournament.startDate)} – {formatDate(tournament.endDate)}</span><span><b className="status live">● Live</b></span><div className="progress-cell"><div className="progress"><i style={{ width: `${45 + index * 17}%` }} /></div><small>{45 + index * 17}%</small></div></div>) : <EmptyState label="No active tournaments found." />}</div></section></div>
}

function Stat({ icon, label, value, trend, color }: { icon: ReactNode; label: string; value: string; trend: string; color: string }) {
  return <div className="stat-card"><div className={`stat-icon ${color}`}>{icon}</div><div className="stat-copy"><span>{label}</span><strong>{value}</strong><small><b>↗ {trend}</b> <em>vs last month</em></small></div></div>
}
function EmptyState({ label }: { label: string }) { return <div className="empty-state"><Swords size={19} /><span>{label}</span></div> }

function ResourcePage({ resource, onToast, onCreateTournament }: { resource: string; onToast: (toast: { type: 'success' | 'error'; message: string }) => void; onCreateTournament: () => void }) {
  const [items, setItems] = useState<Array<Record<string, unknown>>>([])
  const [loading, setLoading] = useState(true)
  const endpoint = resource === 'Tournaments' ? 'tournaments' : resource === 'Matches' ? 'matches' : resource === 'Games' ? 'games' : 'teams'
  useEffect(() => {
    const load = async () => {
      try {
        const { data } = await api.get<Paged<Record<string, unknown>>>(`/${endpoint}?page=1&pageSize=100`)
        setItems(data.items)
      } catch { onToast({ type: 'error', message: `Could not load ${resource.toLowerCase()}.` }) }
      finally { setLoading(false) }
    }
    void load()
  }, [endpoint, onToast, resource])
  const title = resource === 'Teams & Players' ? 'Teams' : resource
  return <div className="page"><div className="page-heading"><div><p className="eyebrow">WORKSPACE / {resource.toUpperCase()}</p><h1>{title}</h1><p className="muted">Manage your {resource.toLowerCase()} from one place.</p></div>{resource === 'Tournaments' && <button className="primary-button" onClick={onCreateTournament}><span>+</span> Create tournament</button>}</div><section className="panel resource-panel">{loading ? <EmptyState label="Loading data…" /> : items.length === 0 ? <EmptyState label={`No ${resource.toLowerCase()} found.`} /> : <div className="resource-list">{items.map((item, index) => <div className="resource-row" key={String(item.id ?? index)}><div className="game-icon game-0"><Gamepad2 size={16} /></div><div><strong>{String(item.name ?? item.gamertag ?? `${item.teamAName ?? 'Team A'} vs ${item.teamBName ?? 'Team B'}`)}</strong><small>{resource === 'Tournaments' && item.startDate ? `${formatDate(String(item.startDate))} – ${formatDate(String(item.endDate))}` : resource === 'Games' ? String(item.description ?? 'Game catalog entry') : `ID #${String(item.id)}`}</small></div><span className="resource-id">#{String(item.id)}</span></div>)}</div>}</section></div>
}

function UserManagementPage({ onToast }: { onToast: (toast: { type: 'success' | 'error'; message: string }) => void }) {
  const [users, setUsers] = useState<User[]>([])
  const [roles, setRoles] = useState<string[]>([])
  const [editing, setEditing] = useState<User | null>(null)
  const [formOpen, setFormOpen] = useState(false)
  const [loading, setLoading] = useState(true)

  const load = useCallback(async () => {
    try {
      const [usersResponse, rolesResponse] = await Promise.all([
        api.get<User[]>('/users'),
        api.get<string[]>('/roles'),
      ])
      setUsers(usersResponse.data)
      setRoles(rolesResponse.data)
    } catch (error) {
      const message = axios.isAxiosError(error) ? String(error.response?.data?.detail ?? 'Users could not be loaded.') : 'Users could not be loaded.'
      onToast({ type: 'error', message })
    } finally { setLoading(false) }
  }, [onToast])

  useEffect(() => {
    const timer = window.setTimeout(() => { void load() }, 0)
    return () => window.clearTimeout(timer)
  }, [load])

  const remove = async (user: User) => {
    if (!window.confirm(`Delete ${user.userName}?`)) return
    try {
      await api.delete(`/users/${user.id}`)
      setUsers((current) => current.filter((item) => item.id !== user.id))
      onToast({ type: 'success', message: 'User deleted successfully.' })
    } catch (error) {
      const message = axios.isAxiosError(error) ? String(error.response?.data?.detail ?? 'User could not be deleted.') : 'User could not be deleted.'
      onToast({ type: 'error', message })
    }
  }

  return <div className="page"><div className="page-heading"><div><p className="eyebrow">MANAGE / USERS</p><h1>Users</h1><p className="muted">Create, update, and remove accounts and roles.</p></div><button className="primary-button" onClick={() => { setEditing(null); setFormOpen(true) }}><span>+</span> Add user</button></div><section className="panel resource-panel">{loading ? <EmptyState label="Loading users…" /> : users.length === 0 ? <EmptyState label="No users found." /> : <div className="users-list">{users.map((user) => <div className="user-row" key={user.id}><div className="avatar user-avatar">{(user.userName[0] ?? 'U').toUpperCase()}</div><div className="user-copy"><strong>{user.userName}</strong><small>{user.email}</small></div><span className="user-role">{user.roles.join(', ') || 'No role'}</span><div className="user-actions"><button className="text-button" onClick={() => { setEditing(user); setFormOpen(true) }}>Edit</button><button className="danger-button" onClick={() => void remove(user)}>Delete</button></div></div>)}</div>}</section>{formOpen && <UserForm user={editing} roles={roles} onClose={() => setFormOpen(false)} onSaved={() => { setFormOpen(false); setLoading(true); void load() }} onToast={onToast} />}</div>
}

function UserForm({ user, roles, onClose, onSaved, onToast }: { user: User | null; roles: string[]; onClose: () => void; onSaved: () => void; onToast: (toast: { type: 'success' | 'error'; message: string }) => void }) {
  const [userName, setUserName] = useState(user?.userName ?? '')
  const [email, setEmail] = useState(user?.email ?? '')
  const [roleName, setRoleName] = useState(user?.roles[0] ?? roles[0] ?? '')
  const [password, setPassword] = useState('')
  const [saving, setSaving] = useState(false)
  const submit = async (event: FormEvent) => {
    event.preventDefault()
    if (!userName.trim() || !email.trim() || !roleName || (!user && !password)) return onToast({ type: 'error', message: 'Complete all required user fields.' })
    setSaving(true)
    try {
      const payload = { userName: userName.trim(), email: email.trim(), roleName, ...(password ? { password } : {}) }
      if (user) await api.put(`/users/${user.id}`, payload)
      else await api.post('/users', payload)
      onToast({ type: 'success', message: user ? 'User updated successfully.' : 'User created successfully.' })
      onSaved()
    } catch (error) {
      const message = axios.isAxiosError(error) ? String(error.response?.data?.detail ?? 'User could not be saved.') : 'User could not be saved.'
      onToast({ type: 'error', message })
    } finally { setSaving(false) }
  }
  return <div className="modal-backdrop" onMouseDown={(event) => { if (event.target === event.currentTarget) onClose() }}><section className="modal" role="dialog" aria-modal="true" aria-labelledby="user-form-title"><div className="panel-heading"><div><p className="eyebrow">{user ? 'EDIT ACCOUNT' : 'NEW ACCOUNT'}</p><h3 id="user-form-title">{user ? 'Edit user' : 'Add user'}</h3></div><button className="icon-button" onClick={onClose} aria-label="Close"><X size={18} /></button></div><form className="modal-form" onSubmit={submit}><label>Username<input value={userName} onChange={(event) => setUserName(event.target.value)} /></label><label>Email<input type="email" value={email} onChange={(event) => setEmail(event.target.value)} /></label><label>Role<select value={roleName} onChange={(event) => setRoleName(event.target.value)}>{roles.map((role) => <option value={role} key={role}>{role}</option>)}</select></label><label>{user ? 'New password (optional)' : 'Password'}<input type="password" value={password} onChange={(event) => setPassword(event.target.value)} minLength={8} /></label><div className="modal-actions"><button type="button" className="secondary-button" onClick={onClose}>Cancel</button><button className="primary-button" disabled={saving}>{saving ? 'Saving…' : user ? 'Save changes' : 'Create user'}</button></div></form></section></div>
}

function TournamentForm({ onClose, onToast }: { onClose: () => void; onToast: (toast: { type: 'success' | 'error'; message: string }) => void }) {
  const [games, setGames] = useState<Game[]>([])
  const [name, setName] = useState('')
  const [gameId, setGameId] = useState('')
  const [startDate, setStartDate] = useState('')
  const [endDate, setEndDate] = useState('')
  const [saving, setSaving] = useState(false)
  useEffect(() => { void api.get<Paged<Game>>('/games?page=1&pageSize=100').then(({ data }) => setGames(data.items)).catch(() => onToast({ type: 'error', message: 'Games could not be loaded.' })) }, [onToast])
  const submit = async (event: FormEvent) => {
    event.preventDefault()
    if (!name.trim() || !gameId || !startDate || !endDate) return onToast({ type: 'error', message: 'Complete all tournament fields.' })
    if (new Date(endDate) < new Date(startDate)) return onToast({ type: 'error', message: 'End date must be after the start date.' })
    setSaving(true)
    try { await api.post('/tournaments', { name: name.trim(), gameId: Number(gameId), startDate: new Date(startDate).toISOString(), endDate: new Date(endDate).toISOString() }); onClose(); onToast({ type: 'success', message: 'Tournament created successfully.' }) }
    catch (error) { const message = axios.isAxiosError(error) ? String(error.response?.data?.detail ?? 'You are not authorized to create tournaments.') : 'Tournament creation failed.'; onToast({ type: 'error', message }) }
    finally { setSaving(false) }
  }
  return <div className="modal-backdrop" onMouseDown={(event) => { if (event.target === event.currentTarget) onClose() }}><section className="modal" role="dialog" aria-modal="true" aria-labelledby="create-tournament-title"><div className="panel-heading"><div><p className="eyebrow">NEW COMPETITION</p><h3 id="create-tournament-title">Create tournament</h3></div><button className="icon-button" onClick={onClose} aria-label="Close"><X size={18} /></button></div><form className="modal-form" onSubmit={submit}><label>Name<input value={name} onChange={(event) => setName(event.target.value)} placeholder="Winter Championship" /></label><label>Game<select value={gameId} onChange={(event) => setGameId(event.target.value)}><option value="">Select a game</option>{games.map((game) => <option value={game.id} key={game.id}>{game.name}</option>)}</select></label><div className="date-fields"><label>Start date<input type="datetime-local" value={startDate} onChange={(event) => setStartDate(event.target.value)} /></label><label>End date<input type="datetime-local" value={endDate} onChange={(event) => setEndDate(event.target.value)} /></label></div><div className="modal-actions"><button type="button" className="secondary-button" onClick={onClose}>Cancel</button><button className="primary-button" disabled={saving}>{saving ? 'Creating…' : 'Create tournament'}</button></div></form></section></div>
}

export default App
